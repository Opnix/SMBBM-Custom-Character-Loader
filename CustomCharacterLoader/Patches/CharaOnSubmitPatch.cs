﻿using System;
using Flash2;
using Flash2.Selector.MainGame;
using Flash2.Selector.TimeAttack;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using CustomCharacterLoader.CharacterManager;
using CustomCharacterLoader.PlayerManager;
using Il2CppSystem.Collections.Generic;

namespace CustomCharacterLoader.Patches
{
    // Delegates for OnSubmit() methods
    public static class CharaOnSubmitPatch
    {
        // Delegate that gets the selected character and preps the player loader
        private delegate void Delegate(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData);
        static void OnSubmit(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            SelMgCharaItemData selectedChara = new(itemData);
            int iconID = selectedChara.costumeList[selectedChara.costumeIndex].spriteIcon.GetInstanceID();
            Main.playerLoader.playerType = PlayerLoader.CharacterType.None;

            // check if player selected a custom character
            int index = 0;
            foreach (CustomCharacter chara in Main.characterManager.characters)
            {
                if (iconID == chara.icon.GetInstanceID())
                {
                    Main.playerLoader.playerType = PlayerLoader.CharacterType.Character;
                    Main.playerLoader.playerIndex = index;
                    Main.playerLoader.CheckPlayerType();
                    break;
                }
                index++;
            }
        }

        // Main Game
        private static Delegate mg_CustomSubmit;
        private static Delegate mg_OriginalSubmit;
        public static unsafe void CreateMainGameDetour()
        {
            mg_CustomSubmit = MgOnSubmit;
            var method = typeof(SelMgCharaSelectWindow).GetMethod(nameof(SelMgCharaSelectWindow.onSubmit), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null));
            mg_OriginalSubmit = ClassInjector.Detour.Detour(methodInfo.MethodPointer, mg_CustomSubmit);
        }
        static void MgOnSubmit(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            OnSubmit(_thisPtr, playerIndex, inputLayer, itemData);
            mg_OriginalSubmit(_thisPtr, playerIndex, inputLayer, itemData);
        }

        // Time Attack
        private static Delegate ta_CustomSubmit;
        private static Delegate ta_OriginalSubmit;
        public static unsafe void CreateTimeAttackDetour()
        {
            ta_CustomSubmit = TaOnSubmit;
            var method = typeof(SelTaCharaSelectWindow).GetMethod(nameof(SelTaCharaSelectWindow.onSubmit), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null));
            ta_OriginalSubmit = ClassInjector.Detour.Detour(methodInfo.MethodPointer, ta_CustomSubmit);
        }
        static void TaOnSubmit(IntPtr _thisPtr, IntPtr playerIndex, IntPtr inputLayer, IntPtr itemData)
        {
            OnSubmit(_thisPtr, playerIndex, inputLayer, itemData);
            ta_OriginalSubmit(_thisPtr, playerIndex, inputLayer, itemData);
        }
    }
}
