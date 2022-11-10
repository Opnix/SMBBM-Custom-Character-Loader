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
        // Character Path
        public static string PATH = "";

        // Scene Name
        private static string sceneName;

        // Custom Character Stuff
        private static CustomCharacterManager customCharacterManager = null;

        // Visual Game Objects (name banner, thumbnails, etc) 
        private static SelMainThemeBeltView displayView = null;
                
        // Mod Load
        public static void OnModLoad(Dictionary<string, object> settings)
        {
        }

        // Mod Start
        public static void OnModStart()
        {
            PATH = System.Reflection.Assembly.GetCallingAssembly().Location.Split(new string[] { "BananaModManager.Loader.IL2Cpp.dll" }, System.StringSplitOptions.None)[0] + "mods\\CustomCharacterLoader\\Characters";
            
            ClassInjector.RegisterTypeInIl2Cpp(typeof(CustomCharacterManager));
            var obj = new GameObject { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(obj);
            customCharacterManager = new CustomCharacterManager(obj.AddComponent(Il2CppType.Of<CustomCharacterManager>()).Pointer, PATH);
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

            // Scene = Title
            if (sceneName == "Title")
            {
            }
            // Scene = MainMenu
            else if (sceneName == "MainMenu")
            {
                // Import characters
                if (!customCharacterManager.importedCharacters)
                {
                    // Create chara item data for character select
                    SelMgCharaItemDataListObject[] charaDataListContainer = Resources.FindObjectsOfTypeAll<SelMgCharaItemDataListObject>();
                    if (charaDataListContainer != null && charaDataListContainer.Length > 0)
                    {
                        customCharacterManager.ImportCharacters(charaDataListContainer[0]);
                    }
                }
                // After Characters are imported
                else
                {
                    // Get belt view if null
                    if (displayView == null)
                    {
                        SelMainThemeBeltView[] views = Object.FindObjectsOfType<SelMainThemeBeltView>();
                        foreach (SelMainThemeBeltView i in views)
                        {
                            if (i.GetThumbnail() != null)
                            {
                                displayView = i;
                                break;
                            }
                        }
                    }
                    // Update images
                    else
                    {
                        customCharacterManager.update(displayView);
                    }
                }
            }
            // Scene = MainGame
            else if (sceneName == "MainGame")
            {
                // Check if custom character is selected
                if (customCharacterManager.checkSelectedCharacter)
                {
                    customCharacterManager.IsCustomCharacterSelected();
                }
            }
        }
    }
}