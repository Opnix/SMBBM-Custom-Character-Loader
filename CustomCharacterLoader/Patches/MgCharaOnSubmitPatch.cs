using System;
using Flash2;
using Flash2.Selector.MainGame;
using Flash2.Selector.TimeAttack;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace CustomCharacterLoader.Patches
{
    // Patch for the onSubmit() function so the mod can know what character was selected.
    public static class MgCharaOnSubmitPatch
    {
        private delegate void Delegate(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData);
        private static Delegate customSubmit;
        private static Delegate originalSubmit;

        public static bool isCustomCharacter { get; set; } = false;
        public static int selectedCharacterID { get; set; }
        public static unsafe void CreateDetour()
        {
            customSubmit = OnSubmit;

            var method = typeof(SelMgCharaSelectWindow).GetMethod(nameof(SelMgCharaSelectWindow.onSubmit), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null));

            originalSubmit = ClassInjector.Detour.Detour(methodInfo.MethodPointer, customSubmit);
        }

        static void OnSubmit(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
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
                    selectedChara.costumeIndex = 0; //LOOK INTO
                    break;
                }
            }

            if (isCustomCharacter) { originalSubmit(_thisPtr, playerIndex, inputLayer, selectedChara.Pointer); }
            else { originalSubmit(_thisPtr, playerIndex, inputLayer, itemData); }
        }
    }
}
