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
    // Delegate for LoadCueSheetASync() method
    public static class MonkeyMutePatch
    {
        // Included dlc characters names because idk what they are
        public static string[] mutedSounds = { "vo_aiai", "vo_baby", "vo_doctor", "vo_gongon", "vo_jam", "vo_jet", "vo_meemee", "vo_yanyan", "vo_beat", "vo_kiryu",
            "vo_sonic", "vo_tails", "vo_dlc01", "vo_dlc02", "vo_dlc03" , "vo_dreamcast", "vo_gamegear", "vo_segasaturn", "vo_suezo", "vo_hellokitty", "vo_morgana"};

        private delegate void Delegate(IntPtr _thisPtr, sound_id.cuesheet in_cueSheet);
        static void LoadCueSheetASync(IntPtr _thisPtr, sound_id.cuesheet in_cueSheet)
        {
            Sound __instance = new(_thisPtr);
            sound_id.cuesheet send = in_cueSheet;
            foreach (string sound in mutedSounds)
            {
                if (in_cueSheet.ToString() == sound & Main.playerLoader.playerType != PlayerManager.PlayerLoader.CharacterType.None)
                {
                    Console.WriteLine("Muting " + sound);
                    send = sound_id.cuesheet.invalid;
                    break;
                }
            }
            originalLoadCueSheet(_thisPtr, send);
        }

        private static Delegate customLoadCueSheet;
        private static Delegate originalLoadCueSheet;
        public static unsafe void CreateDetour()
        {
            customLoadCueSheet = LoadCueSheetASync;
            var method = typeof(Sound).GetMethod(nameof(Sound.LoadCueSheetASync), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null));
            originalLoadCueSheet = ClassInjector.Detour.Detour(methodInfo.MethodPointer, customLoadCueSheet);
        }
    }
}