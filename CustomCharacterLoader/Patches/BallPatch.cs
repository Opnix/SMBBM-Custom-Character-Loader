using System;
using Flash2;
using Flash2.Selector.MainGame;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace CustomCharacterLoader.Patches
{
    internal class BallPatch
    {
        private delegate CharaCustomize.PartsSet GetPartsSetDelegate(Chara.eKind in_CharaKind);
        private static GetPartsSetDelegate OnSelectInstance;
        private static GetPartsSetDelegate OnSelectOriginal;

        public static unsafe void CreateDetour()
        {
            OnSelectInstance = GetPartsSet;

            var original = typeof(CharaCustomizeDefaultPartsSetData).GetMethod(nameof(CharaCustomizeDefaultPartsSetData.GetPartsSet), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));

            OnSelectOriginal = ClassInjector.Detour.Detour(methodInfo.MethodPointer, OnSelectInstance);
        }
        static CharaCustomize.PartsSet GetPartsSet(Chara.eKind in_CharaKind)
        {
            Console.WriteLine(in_CharaKind);
            CharaCustomize.PartsSet original = new CharaCustomize.PartsSet();
            return original;
        }
    }
}
