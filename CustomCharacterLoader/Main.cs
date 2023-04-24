using System.Collections.Generic;
using Flash2;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System;
using UnhollowerRuntimeLib;
using System.Reflection;

namespace CustomCharacterLoader
{
    public static class Main
    {
        public static string PATH = "";
        public static string GUEST_CHARACTER_PATH = "";
        private static string sceneName;

        // Manager Objects
        public static CustomCharacterManager customCharacterManager = null;
        public static PlayerLoader playerLoader = null;
        private static MonkeyVoices monkeyVoices = null;
        
        // Console Text
        public static void Output(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CustomCharacterLoader: " + text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Mod Load
        public static void OnModLoad(Dictionary<string, object> settings)
        {
        }

        // Mod Start
        public static void OnModStart()
        {
            PATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Characters\");

            // Create custom character manager
            var obj = new GameObject { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(obj);

            ClassInjector.RegisterTypeInIl2Cpp(typeof(CustomCharacterManager));
            customCharacterManager = new CustomCharacterManager(obj.AddComponent(Il2CppType.Of<CustomCharacterManager>()).Pointer, PATH);
            ClassInjector.RegisterTypeInIl2Cpp(typeof(PlayerLoader));            
            playerLoader = new PlayerLoader(obj.AddComponent(Il2CppType.Of<PlayerLoader>()).Pointer);

            // Patches
            Patches.MgCharaOnSubmitPatch.CreateDetour();
            Patches.TaCharaOnSubmitPatch.CreateDetour();
            Patches.MgCharaNamePatch.CreateDetour();
            Patches.TaCharaNamePatch.CreateDetour();
        }

        // On Update
        public static void OnModUpdate()
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName == "MainMenu")
            {
                customCharacterManager.ImportCharacters();
                playerLoader.RevertChanges();
                /*
                // get costume manager 
                else if(customCharacterManager.charaCustomizeManager == null)
                {
                    CharaCustomizeManager[] charaCustomizeContainer = Resources.FindObjectsOfTypeAll<CharaCustomizeManager>();
                    if (charaCustomizeContainer != null && charaCustomizeContainer.Length > 0)
                    {
                        customCharacterManager.charaCustomizeManager = charaCustomizeContainer[0];
                    }    
                }
                */
            }
            else if (sceneName == "MainGame")
            {
                if (playerLoader.loadCharacter)
                {
                    playerLoader.LoadPlayer();
                }
            }
        }

    }
}