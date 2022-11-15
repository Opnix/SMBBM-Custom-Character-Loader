using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash2;
using UnityEngine;

namespace CustomCharacterLoader
{
    class CustomCharacterManager : MonoBehaviour
    {
        public bool importedCharacters = false;
        public List<CustomCharacter> characters = new List<CustomCharacter>();

        public static List<int> BannerID = new List<int>();
        public static List<String> BannerNames = new List<String>();

        public bool loadCharacter = false;
        public CustomCharacter selectedCharacter;
        private static GameObject CustomMonkeyObject;

        public CustomCharacterManager() { }
        public CustomCharacterManager(IntPtr value) : base(value) { }
        public CustomCharacterManager(IntPtr value, string PATH) : base(value)
        {
            // Reads all the Json files in Character folder
            foreach (string json in Directory.GetFiles(PATH, "*.json"))
            {
                StreamReader reader = new StreamReader(json);
                string data = reader.ReadToEnd();

                string name = json.Substring(json.LastIndexOf("\\")+1);
                name = name.Remove(name.Length - 5); // remove .json

                CustomCharacter character = new CustomCharacter(name, data, PATH);

                if (character.asset != null)
                {
                    characters.Add(character);
                }
            }
        }

        // Create Character select icons for all custom characters
        public void ImportCharacters(SelMgCharaItemDataListObject charactersList)
        {
            foreach (CustomCharacter chara in characters)
            {
                chara.CreateItemData(charactersList);
                BannerID.Add(chara.icon.GetInstanceID());
                BannerNames.Add(chara.charaName);
            }
            Console.WriteLine("Created Custom Character Slots.");
            importedCharacters = true;
        }

        // Update Icons
        public void update()
        {
            foreach (CustomCharacter chara in characters)
            {
                // Update Icons
                if (!chara.icon)
                {
                    chara.icon = chara.asset.LoadAsset<Sprite>("icon");
                }
                if (!chara.banner)
                {
                    chara.banner = chara.asset.LoadAsset<Sprite>("banner");
                }
                chara.itemData.costumeList[0].spritePicture = chara.banner;
                chara.itemData.costumeList[0].spriteIcon = chara.icon;
            }
        }

        // Check if a custom character is selected
        public void IsCustomCharacterSelected()
        {
            loadCharacter = false;
            Patches.MgCharaOnSubmitPatch.checkSelectedCharacter = false;
            Patches.TaCharaOnSubmitPatch.checkSelectedCharacter = false;
            foreach (CustomCharacter chara in characters)
            {
                if (Patches.MgCharaOnSubmitPatch.selectedCharacterID == chara.icon.GetInstanceID())
                {
                    selectedCharacter = chara;
                    loadCharacter = true;
                    break;
                }
            }
        }

        public void LoadCustomCharacter()
        {
            // look for player object
            if (CustomMonkeyObject == null)
            {
                UnhollowerBaseLib.Il2CppArrayBase<Monkey> monkey = Resources.FindObjectsOfTypeAll<Monkey>();
                if (monkey.Length > 1)
                {
                    GameObject playerObject = monkey[0].gameObject;

                    GameObject target_model = playerObject.GetComponentInChildren<Animator>().gameObject;
                    Shader shader = playerObject.GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
                    CustomMonkeyObject = selectedCharacter.InitializeCharacter(shader);

                    foreach (var component in playerObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        component.enabled = false;
                    }

                    CustomMonkeyObject.transform.parent = target_model.transform;
                    playerObject.GetComponent<MonkeyRef>().m_Animator = CustomMonkeyObject.GetComponentInChildren<Animator>();
                    playerObject.GetComponent<Monkey>().m_Animator = CustomMonkeyObject.GetComponentInChildren<Animator>();
                }
            }
        }
    }
}
