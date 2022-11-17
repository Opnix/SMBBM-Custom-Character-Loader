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
        private delegate void OnSubmitDelegate(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData);
        private static OnSubmitDelegate OnSubmitInstance;
        private static OnSubmitDelegate OnSubmitOriginal;

        public static int selectedCharacterID;
        public static bool checkSelectedCharacter = true;
        public static unsafe void CreateDetour()
        {
            OnSubmitInstance = onSubmit;

            var original = typeof(SelTaCharaSelectWindow).GetMethod(nameof(SelTaCharaSelectWindow.onSubmit), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));

            OnSubmitOriginal = ClassInjector.Detour.Detour(methodInfo.MethodPointer, OnSubmitInstance);
        }
        static void onSubmit(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            SelMgCharaItemData selectedChara = new(itemData);
            selectedCharacterID = selectedChara.costumeList[selectedChara.costumeIndex].spriteIcon.GetInstanceID();
            checkSelectedCharacter = true;

            OnSubmitOriginal(_thisPtr, playerIndex, inputLayer, itemData);
        }
    }
}
