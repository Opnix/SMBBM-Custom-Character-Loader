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
    // Delegates for OnSelect() methods
    public static class CharaNamePatch
    {
        // Delegate to rename character banner
        private delegate void OnSelectDelegate(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData);
        static void OnSelect(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            SelMgCharaItemData selectedChara = new(itemData);
            SelMgCharaSelectWindow _instance = new(_thisPtr);

            int charaIconId = selectedChara.costumeList[0].spriteIcon.GetInstanceID();
            foreach(CharacterManager.CustomCharacter character in Main.characterManager.characters)
            {
                if(character.icon.GetInstanceID() == charaIconId)
                {
                    _instance.themeBeltView.SetCharacterName(character.charaName);
                    break;
                }    
            }
        }

        // Main Game
        private static OnSelectDelegate mg_CustomSelect;
        private static OnSelectDelegate mg_OriginalSelect;
        public static unsafe void CreateMainGameDetour()
        {
            mg_CustomSelect = MgOnSelect;
            var original = typeof(SelMgCharaSelectWindow).GetMethod(nameof(SelMgCharaSelectWindow.onSelect), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));
            mg_OriginalSelect = ClassInjector.Detour.Detour(methodInfo.MethodPointer, mg_CustomSelect);
        }
        static void MgOnSelect(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            mg_OriginalSelect(_thisPtr, playerIndex, inputLayer, itemData);
            OnSelect(_thisPtr, playerIndex, inputLayer, itemData);
        }

        // Time Attack
        private static OnSelectDelegate ta_CustomSelect;
        private static OnSelectDelegate ta_OriginalSelect;
        public static unsafe void CreateTimeAttackDetour()
        {
            ta_CustomSelect = TaOnSelect;
            var original = typeof(SelTaCharaSelectWindow).GetMethod(nameof(SelTaCharaSelectWindow.onSelect), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));
            ta_OriginalSelect = ClassInjector.Detour.Detour(methodInfo.MethodPointer, ta_CustomSelect);
        }
        static void TaOnSelect(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            ta_OriginalSelect(_thisPtr, playerIndex, inputLayer, itemData);
            OnSelect(_thisPtr, playerIndex, inputLayer, itemData);
        }
    }
}
