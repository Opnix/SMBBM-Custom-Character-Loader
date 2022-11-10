using System;
using Flash2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace CustomCharacterLoader
{
    class CharacterSelectPatch
    {
        private delegate SelGridViewBase<SelMgCharaItemData, SelDiagonalGridContext>.SubmitEvent OnSubmitDelegate(IntPtr _thisPtr);
        private static OnSubmitDelegate OnSubmitDelegateInstance;
        private static OnSubmitDelegate OnSubmitDelegateOriginal;

        public static unsafe void CreateDetour()
        {
            OnSubmitDelegateInstance = LoadCueSheetASync;

            var original = typeof(SelMgCharaSelectView).GetMethod(nameof(SelMgCharaSelectView.onSubmit), AccessTools.all);
            var methodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(original).GetValue(null));

            OnSubmitDelegateOriginal = ClassInjector.Detour.Detour(methodInfo.MethodPointer, OnSubmitDelegateInstance);
        }

        static SelGridViewBase<SelMgCharaItemData, SelDiagonalGridContext>.SubmitEvent LoadCueSheetASync(IntPtr _thisPtr)
        {
            SelMgCharaSelectView __instance = new(_thisPtr);

            return OnSubmitDelegateOriginal(_thisPtr);
        }
    }
}
