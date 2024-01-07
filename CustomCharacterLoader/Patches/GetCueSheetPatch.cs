using System;
using Flash2;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace CustomCharacterLoader.Patches
{
    // Delegates for GetCueSheet
    public static class GetCueSheetPatch
    {
        // Delegate to rename character banner
        private delegate sound_id.cuesheet GetCueSheetDelegate(IntPtr _thisPtr, Chara.eKind in_charaKind);

        // Main Game
        private static GetCueSheetDelegate customMethod;
        private static GetCueSheetDelegate originalMethod;
        public static unsafe void CreateGetCueSheetDetour()
        {
            customMethod = GetCueSheet;
            var original = typeof(Sound.voice_param_t).GetMethod(nameof(Sound.voice_param_t.GetCueSheet), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));
            originalMethod = ClassInjector.Detour.Detour(methodInfo.MethodPointer, customMethod);
        }
        static sound_id.cuesheet GetCueSheet(IntPtr _thisPtr, Chara.eKind in_charaKind)
        {
            if(Main.playerLoader.playerType != PlayerManager.PlayerLoader.CharacterType.None && Main.playerLoader.selectedCharacter.monkey_voice_id != 0)
            {
                return (sound_id.cuesheet)Main.playerLoader.selectedCharacter.monkey_voice_id;
            }
            else
            {
                return originalMethod(_thisPtr, in_charaKind);
            }
        }
    }
    public static class testPatch
    {
        // Delegate to rename character banner
        private delegate void handle_play_delegate(IntPtr _thisPtr, IntPtr cuesheetname, IntPtr cuename);

        // Main Game
        private static handle_play_delegate customMethod;
        private static handle_play_delegate originalMethod;
        public static unsafe void CreateGetCueSheetDetour()
        {
            customMethod = handle_play;
            var original = typeof(Sound.handle_param_t).GetMethod(nameof(Sound.handle_param_t.Play), new Type[] { typeof(String), typeof(String) });
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));
            originalMethod = ClassInjector.Detour.Detour(methodInfo.MethodPointer, customMethod);
        }
        static void handle_play(IntPtr _thisPtr, IntPtr cuesheetname, IntPtr cuename)
        {
            string cuenameString = IL2CPP.Il2CppStringToManaged(cuename);
            string cuesheetnameString = IL2CPP.Il2CppStringToManaged(cuesheetname);
            Console.WriteLine("BAAAAA "+cuesheetnameString + ", " + cuenameString);
            originalMethod(_thisPtr, cuesheetname, cuename);
        }
    }
}