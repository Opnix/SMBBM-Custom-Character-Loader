using System;
using Flash2;
using Flash2.Selector.MainGame;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace CustomCharacterLoader.Patches
{
    // This Patches the SelMgCharaSelectWindow.onSelect() and updates the name banner text with a custom character's
    public static class MgCharaNamePatch
    {
        private delegate void onSelectDelegate(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData);
        private static onSelectDelegate OnSelectInstance;
        private static onSelectDelegate OnSelectOriginal;

        public static unsafe void CreateDetour()
        {
            OnSelectInstance = onSelect;

            var original = typeof(SelMgCharaSelectWindow).GetMethod(nameof(SelMgCharaSelectWindow.onSelect), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));

            OnSelectOriginal = ClassInjector.Detour.Detour(methodInfo.MethodPointer, OnSelectInstance);
        }
        static void onSelect(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            OnSelectOriginal(_thisPtr, playerIndex, inputLayer, itemData);
            SelMgCharaItemData selectedChara = new(itemData);
            SelMgCharaSelectWindow _instance = new(_thisPtr);

            int index = 0;
            foreach (int customIconID in CustomCharacterManager.BannerID)
            {
                if (customIconID == selectedChara.costumeList[0].spriteIcon.GetInstanceID())
                {
                    _instance.themeBeltView.SetCharacterName(CustomCharacterManager.BannerNames[index]);
                    break;
                }
                index++;
            }
        }
    }
}
