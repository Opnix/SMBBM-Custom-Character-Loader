using System.Collections.Generic;
using Flash2;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System;
using UnhollowerRuntimeLib;

namespace CustomCharacterLoader
{
    public static class Main
    {
        public static string PATH = "";
        private static string sceneName;
        private static CustomCharacterManager customCharacterManager = null;
                
        // Mod Load
        public static void OnModLoad(Dictionary<string, object> settings)
        {
        }

        // Mod Start
        public static void OnModStart()
        {
            PATH = System.Reflection.Assembly.GetCallingAssembly().Location.Split(new string[] { "BananaModManager.Loader.IL2Cpp.dll" }, System.StringSplitOptions.None)[0] + "mods\\CustomCharacterLoader\\Characters";

            // create custom character manager
            ClassInjector.RegisterTypeInIl2Cpp(typeof(CustomCharacterManager));
            var obj = new GameObject { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(obj);
            customCharacterManager = new CustomCharacterManager(obj.AddComponent(Il2CppType.Of<CustomCharacterManager>()).Pointer, PATH);
            
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
            }

            // Scene = MainMenu
            if (sceneName == "MainMenu")
            {
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

                        customCharacterManager.update(); // make sure pictures stay
                    }
                }
                // After Characters are imported
                else
                {
                    customCharacterManager.update(); // really make sure pictures stay
                }
            }
            // Scene = MainGame
            else if (sceneName == "MainGame")
            {
                // Check if custom character is selected
                if (Patches.MgCharaOnSubmitPatch.checkSelectedCharacter || Patches.TaCharaOnSubmitPatch.checkSelectedCharacter)
                {
                    customCharacterManager.IsCustomCharacterSelected();
                }
            }
        }

        public static void OnModLateUpdate()
        {
            if (sceneName == "MainMenu")
            {
                if(customCharacterManager.importedCharacters)
                {
                    customCharacterManager.update(); // really REALLY make sure pictures stay...
                }
            }
        }
    }
}