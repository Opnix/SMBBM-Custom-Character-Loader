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
        public static CustomCharacterManager customCharacterManager = null;
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

            // create custom character manager
            ClassInjector.RegisterTypeInIl2Cpp(typeof(CustomCharacterManager));
            
            var obj = new GameObject { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(obj);
            customCharacterManager = new CustomCharacterManager(obj.AddComponent(Il2CppType.Of<CustomCharacterManager>()).Pointer, PATH);

            // Get Guest Character Pack assembly path
            Assembly assembly = Assembly.GetCallingAssembly();
            Type loader = assembly.GetType("BananaModManager.Loader.IL2Cpp.Loader");
            PropertyInfo infoList = loader.GetProperty("Mods");
            List<BananaModManager.Shared.Mod> mods = (List<BananaModManager.Shared.Mod>)infoList.GetValue(loader);
            foreach (var mod in mods)
            {
                string modName = mod.GetAssembly().ToString().Split(',')[0];
                if (modName == "GuestCharacters")
                {
                    
                }
            }

            // Patches
            Patches.MgCharaOnSubmitPatch.CreateDetour();
            Patches.TaCharaOnSubmitPatch.CreateDetour();
        }

        // Mod Late Update (Split by scene names)
        public static void OnModUpdate()
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // Load Player Models when in Main Game
            if (customCharacterManager.loadCharacter)
            {
                customCharacterManager.LoadCustomCharacter();
                if(monkeyVoices != null)
                {
                    monkeyVoices.LoadSounds(customCharacterManager.selectedCharacter);
                }
            }

            // Scene = MainMenu
            if (sceneName == "MainMenu")
            {
                customCharacterManager.loadCharacter = false;
                // Import characters
                if (!customCharacterManager.importedCharacters)
                {
                    // Create chara item data for character select
                    SelMgCharaItemDataListObject[] charaDataListContainer = Resources.FindObjectsOfTypeAll<SelMgCharaItemDataListObject>();
                    if (charaDataListContainer != null && charaDataListContainer.Length > 0)
                    {
                        customCharacterManager.ImportCharacters(charaDataListContainer[0]);

                        // patch for name banner now that all the characters are loaded
                        Patches.MgCharaNamePatch.CreateDetour();
                        Patches.TaCharaNamePatch.CreateDetour();

                        customCharacterManager.Update(); // make sure pictures stay
                    }
                }
                // get costume manager 
                else if(customCharacterManager.charaCustomizeManager == null)
                {
                    CharaCustomizeManager[] charaCustomizeContainer = Resources.FindObjectsOfTypeAll<CharaCustomizeManager>();
                    if (charaCustomizeContainer != null && charaCustomizeContainer.Length > 0)
                    {
                        customCharacterManager.charaCustomizeManager = charaCustomizeContainer[0];
                    }    
                }
                // After Characters are imported
                else
                {
                    customCharacterManager.RevertChanges();
                    customCharacterManager.Update(); // really make sure pictures stay
                }
            }
            // Scene = MainGame
            else if (sceneName == "MainGame")
            {
                // Get the character data if custom character is selected
                if (Patches.MgCharaOnSubmitPatch.isCustomCharacter || Patches.TaCharaOnSubmitPatch.isCustomCharacter)
                {
                    customCharacterManager.GetCustomCharacterSelected();
                }
            }
        }

        public static void OnModLateUpdate()
        {
            if (sceneName == "MainMenu")
            {
                if (customCharacterManager.importedCharacters)
                {
                    customCharacterManager.Update(); // really REALLY make sure pictures stay...
                }
            }
        }
    }
}