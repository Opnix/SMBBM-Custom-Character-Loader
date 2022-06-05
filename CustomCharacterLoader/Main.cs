using System.Collections.Generic;
using Flash2;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System;

namespace CustomCharacterLoader
{
    public static class Main
    {
        // Character Path
        public static string PATH = "";

        // Scene Name
        private static string sceneName;

        // Booleans for Initializing stuff
        private static bool initiate = true;
        private static bool startup = true;

        // Boolean to check if player chose custom character
        private static bool checkSelectedCharacter = true;

        // Custom Character Stuff
        private static List<CustomCharacter> characterList = new List<CustomCharacter>();

        private static int selectedCharacterID = 0;
        private static CustomCharacter selectedCharacter;
        private static bool loadCharacter = false;
        private static GameObject modModel;

        // Visual Game Objects (name banner, thumbnails, etc) 
        private static SelMainThemeBeltView displayView = null;

        // Reads all the Json files in /Character and inserts the character data in a list
        private static void ReadCharacterFolder()
        {
            foreach (string json in Directory.GetFiles(PATH, "*.json"))
            {
                StreamReader reader = new StreamReader(json);
                string data = reader.ReadToEnd();
                CustomCharacter character = new CustomCharacter(data, PATH);

                if (character.asset != null)
                {
                    characterList.Add(character);
                }
            }
        }

        // Mod Load
        public static void OnModLoad(Dictionary<string, object> settings)
        {
        }

        // Mod Start
        public static void OnModStart()
        {
            PATH = System.Reflection.Assembly.GetCallingAssembly().Location.Split(new string[] { "BananaModManager.Loader.IL2Cpp.dll" }, System.StringSplitOptions.None)[0] + "mods\\CustomCharacterLoader\\Characters";
        }

        // Mod Late Update
        public static void OnModLateUpdate()
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // Load Player Models when in Main Game
            if (loadCharacter == true)
            {
                // look for player object
                if (modModel == null)
                {                    
                    UnhollowerBaseLib.Il2CppArrayBase<Monkey> monkey = Resources.FindObjectsOfTypeAll<Monkey>();
                    if (monkey.Length > 1)
                    {
                        GameObject playerObject = monkey[0].gameObject;

                        GameObject target_model = playerObject.GetComponentInChildren<Animator>().gameObject;
                        Shader shader = playerObject.GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
                        modModel = selectedCharacter.InitializeCharacter(shader);

                        foreach (var component in playerObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            component.enabled = false;
                        }

                        modModel.transform.parent = target_model.transform;
                        playerObject.GetComponent<MonkeyRef>().m_Animator = modModel.GetComponentInChildren<Animator>();
                        playerObject.GetComponent<Monkey>().m_Animator = modModel.GetComponentInChildren<Animator>();
                    }
                }
            }

            // when scene is in MainGame
            if (sceneName== "MainGame")
            {
                
                if(checkSelectedCharacter)
                {
                    checkSelectedCharacter = false;
                    foreach (CustomCharacter chara in characterList)
                    {
                        if (selectedCharacterID == chara.picture.GetInstanceID())
                        {
                            selectedCharacter = chara;
                            loadCharacter = true;
                        }
                    }
                }
            }
            // when scene is in Title
            else if(sceneName == "Title")
            {
                // read jsons in character folder
                if(startup)
                {
                    startup = false;
                    ReadCharacterFolder();
                }
            }
            // when scene is in the MainMenu
            else if(sceneName == "MainMenu")
            {
                loadCharacter = false;
                checkSelectedCharacter = true;

                // Load Essential Parts
                if (initiate)
                {
                    // create chara item data for character select
                    SelMgCharaItemDataListObject[] charaDataListContainer = Resources.FindObjectsOfTypeAll<SelMgCharaItemDataListObject>();
                    if (charaDataListContainer != null && charaDataListContainer.Length > 0)
                    {
                        initiate = false;
                        foreach (CustomCharacter chara in characterList)
                        {
                            chara.CreateItemData(charaDataListContainer[0]);
                        }
                        Console.WriteLine("Created Custom Character Select Slots.");
                    }
                }
                // After Essentials are finished
                else
                {
                    // Edit the Character Select Details
                    foreach (CustomCharacter chara in characterList)
                    {
                        // Update Icons
                        if (!chara.icon)
                        {
                            chara.icon = chara.asset.LoadAsset<Sprite>("icon");
                        }
                        if (!chara.picture)
                        {
                            chara.picture = chara.asset.LoadAsset<Sprite>("picture");
                        }
                        chara.itemData.costumeList[0].spritePicture = chara.picture;
                        chara.itemData.costumeList[0].spriteIcon = chara.icon;

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
                        else
                        {
                            // if there's an image thumbnail
                            if(displayView.GetThumbnail())
                            {
                                // get last selected character
                                if (!displayView.GetThumbnail().name.StartsWith("t_tmb"))
                                {
                                    selectedCharacterID = displayView.GetThumbnail().GetInstanceID();
                                }
                                // Update Name Banner
                                if (displayView.GetThumbnail().GetInstanceID() == chara.picture.GetInstanceID())
                                {
                                    displayView.SetCharacterName(chara.charaName);
                                }
                            }
                        }
                        
                    }
                }                
            }
        }
    }
}