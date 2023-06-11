using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash2;
using UnityEngine;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace CustomCharacterLoader.CharacterManager
{
    public class CustomCharacterManager : MonoBehaviour
    {
        // Lists of character stuff
        public Dictionary<int, string> BannerDict = new Dictionary<int, string>();
        public List<CustomCharacter> characters = new List<CustomCharacter>();

        // Force character import
        public static bool importedCharacters = false;
        
        public CustomCharacterManager() { }
        public CustomCharacterManager(IntPtr ptr) : base(ptr) { }
        public CustomCharacterManager(IntPtr ptr, string path) : base(ptr)
        {
            // Go to each folder in Character folder
            foreach (string dir in Directory.GetDirectories(path))
            {
                Console.WriteLine(dir);
                // Reads all the Json files in folder
                foreach (string json in Directory.GetFiles(dir, "*.json"))
                {
                    StreamReader reader = new StreamReader(json);
                    string data = reader.ReadToEnd();
                    string name = dir.Substring(dir.LastIndexOf("\\") + 1);
                    CustomCharacter character = new CustomCharacter(name, data, dir);
                    reader.Close();

                    if (character.asset != null)
                    {
                        characters.Add(character);
                    }
                }
            }
        }

        // Create character select icons for all custom characters
        public void Load()
        {
            if (!importedCharacters)
            {
                SelMgCharaItemDataListObject[] gameCharacterList = Resources.FindObjectsOfTypeAll<SelMgCharaItemDataListObject>();
                if (gameCharacterList != null && gameCharacterList.Length > 0)
                {
                    foreach (CustomCharacter chara in characters)
                    {
                        chara.CreateItemData(gameCharacterList[0]);
                        BannerDict.Add(chara.icon.GetInstanceID(), chara.charaName);
                    }
                    Main.Output("Created Custom Character Slots.");
                    importedCharacters = true;
                }
            }
            Update(); // Make sure pictures stay
        }

        // Update icons
        public void Update()
        {
            foreach (CustomCharacter chara in characters)
            {
                chara.UpdateCharacterSelect();
            }
        }
        
        // Replace ingame model
        public static GameObject ReplaceModel(CustomCharacter selectedCharacter)
        {
            GameObject customPlayerObject = null;
            Il2CppArrayBase<Monkey> monkey = Resources.FindObjectsOfTypeAll<Monkey>();
            if (monkey.Length > 1)
            {
                GameObject playerObject = monkey[0].gameObject;

                GameObject target_model = playerObject.GetComponentInChildren<Animator>().gameObject;
                Shader shader = playerObject.GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
                customPlayerObject = selectedCharacter.InitializeCharacter(shader);

                foreach (var component in playerObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    component.enabled = false;
                }

                customPlayerObject.transform.parent = target_model.transform;
                playerObject.GetComponent<MonkeyRef>().m_Animator = customPlayerObject.GetComponentInChildren<Animator>();
                playerObject.GetComponent<Monkey>().m_Animator = customPlayerObject.GetComponentInChildren<Animator>();
            }
            return customPlayerObject;
        }
    }
}
