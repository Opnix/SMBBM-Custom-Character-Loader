using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.IO;
using System;
using UnhollowerRuntimeLib;
using System.Reflection;
using CustomCharacterLoader.CharacterManager;
using CustomCharacterLoader.Patches;
using CustomCharacterLoader.PlayerManager;

namespace CustomCharacterLoader
{
    public static class Main
    {
        // File Paths
        public static string PATH = "";
        public static string DYNAMIC_SOUNDS_PATH = "";
        public static string GUEST_CHARACTER_PATH = "";

        // Mod Objects
        public static CustomCharacterList customCharacterManager = null;
        public static PlayerLoader playerLoader = null;

		// Asset Bundle
		private static AssetBundle assetBundle;
		public static Shader CustomShader { get; private set; }

		// Console Text
		public static void Output(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CustomCharacterLoader: " + text);
            Console.ForegroundColor = ConsoleColor.White;
        }

		// Mod Load
		// Mod Load
		public static void OnModLoad(Dictionary<string, object> settings)
		{
		}

		private static void LoadAssetBundle()
		{
			if (assetBundle == null)
			{
				Output("Loading AssetBundle...");
				string assemblyLocation = Assembly.GetExecutingAssembly().Location;
				string DLLFolder = Path.GetDirectoryName(assemblyLocation);
				string assetBundlePath = Path.Combine(DLLFolder, "sbmshader");
                Output("AssetBundle path: " + assetBundlePath);
				assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

				if (assetBundle != null)
				{
					Output("AssetBundle loaded successfully.");
				}
				else
				{
					Output("Failed to load AssetBundle.");
				}
			}
			else
			{
				Output("AssetBundle already loaded.");
			}
		}



		// Mod Start
		public static void OnModStart()
        {
			// Get Paths
			PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Assembly assembly = Assembly.GetCallingAssembly();
            Type loader = assembly.GetType("BananaModManager.Loader.IL2Cpp.Loader");
            PropertyInfo infoList = loader.GetProperty("Mods");
            List<BananaModManager.Shared.Mod> mods = (List<BananaModManager.Shared.Mod>)infoList.GetValue(loader);
            foreach (var mod in mods)
            {
                string modName = mod.GetAssembly().ToString().Split(',')[0];
                if (modName == "Dynamic Sounds (Main Cast)")
                {
                    Main.DYNAMIC_SOUNDS_PATH = mod.Directory.ToString();
                    Main.Output("Found Dynamic Sounds Path");
                }
                else if(modName == "GuestCharacters")
                {
                    Main.GUEST_CHARACTER_PATH = mod.Directory.ToString();
                    Main.Output("Found Guest Characters Path");
                }
            }

            // Create il2cpp objects
            var obj = new GameObject { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(obj);

            ClassInjector.RegisterTypeInIl2Cpp(typeof(CustomCharacterList));
            Main.customCharacterManager = new CustomCharacterList(obj.AddComponent(Il2CppType.Of<CustomCharacterList>()).Pointer, Path.Combine(Main.PATH, "Characters\\"));

            ClassInjector.RegisterTypeInIl2Cpp(typeof(PlayerLoader));
            Main.playerLoader = new PlayerLoader(obj.AddComponent(Il2CppType.Of<PlayerLoader>()).Pointer);

            // Create detours
            CharaOnSubmitPatch.CreateMainGameDetour();
            CharaOnSubmitPatch.CreateTimeAttackDetour();
            CharaNamePatch.CreateMainGameDetour();
            CharaNamePatch.CreateTimeAttackDetour();
            MonkeyMutePatch.CreateDetour();
        }

        // Mod Update (Split by scene names)
        public static string sceneName;
        public static bool loadCharacter = false;
        public static void OnModUpdate()
        {
			// Load character. scene names change per level... I don't care to keep track of all those names...
			LoadAssetBundle();
			if (loadCharacter)
            {
                Main.playerLoader.LoadPlayer(assetBundle);
            }

            Main.sceneName = SceneManager.GetActiveScene().name;
            if (Main.sceneName == "MainMenu")
            {
                Main.loadCharacter = false;
                Main.playerLoader.Load();
                try { Main.customCharacterManager.Load(); }
                catch (Exception) { } // The mode likes to throw unimportant errors when this method loads too fast.
            }
            else if (Main.sceneName == "MainGame")
            {
                Main.loadCharacter = true;
            }
        }
    }
}