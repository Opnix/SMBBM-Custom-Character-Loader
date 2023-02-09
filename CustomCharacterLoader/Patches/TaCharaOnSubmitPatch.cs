using System;
using Flash2;
using Flash2.Selector.TimeAttack;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace CustomCharacterLoader.Patches
{
    public static class TaCharaOnSubmitPatch
    {
        private delegate void Delegate(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData);
        private static Delegate customSubmit;
        private static Delegate originalSubmit;

        public static bool isCustomCharacter { get; set; }
        public static int selectedCharacterID { get; set; }
        public static unsafe void CreateDetour()
        {
            customSubmit = HandleSubmit;

            var method = typeof(SelTaCharaSelectWindow).GetMethod(nameof(SelTaCharaSelectWindow.onSubmit), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null));

            originalSubmit = ClassInjector.Detour.Detour(methodInfo.MethodPointer, customSubmit);
        }
        static void HandleSubmit(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            SelMgCharaItemData selectedChara = new(itemData);

            // check if character is a custom character
            isCustomCharacter = false;
            selectedCharacterID = selectedChara.costumeList[selectedChara.costumeIndex].spriteIcon.GetInstanceID();
            foreach (CustomCharacter chara in Main.customCharacterManager.characters)
            {
                if (selectedCharacterID == chara.icon.GetInstanceID())
                {
                    isCustomCharacter = true;
                    selectedCharacterID = selectedChara.costumeList[selectedChara.costumeIndex].spriteIcon.GetInstanceID();
                    selectedChara.costumeIndex = 0;
                    break;
                }
            }

            if (isCustomCharacter) { originalSubmit(_thisPtr, playerIndex, inputLayer, selectedChara.Pointer); }
            else { originalSubmit(_thisPtr, playerIndex, inputLayer, itemData); }
        }
    }
}
