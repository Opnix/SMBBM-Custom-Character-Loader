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
        private List<CustomCharacter> characters = new List<CustomCharacter>();

        public bool checkSelectedCharacter = true;
        private int selectedCharacterID = 0;
        public CustomCharacter selectedCharacter;

        public bool loadCharacter = false;
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

        public void ImportCharacters(SelMgCharaItemDataListObject charactersList)
        {
            foreach (CustomCharacter chara in characters)
            {
                chara.CreateItemData(charactersList);
            }
            Console.WriteLine("Created Custom Character Select Slots.");
            importedCharacters = true;
        }

        public void update(SelMainThemeBeltView characterSelectDisplay)
        {
            checkSelectedCharacter = true;
            loadCharacter = false;

            // Update Icons and Character Name Banner
            foreach (CustomCharacter chara in characters)
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

                // update image thumbnail
                if (characterSelectDisplay.GetThumbnail())
                {
                    // get last selected character
                    if (!characterSelectDisplay.GetThumbnail().name.StartsWith("t_tmb"))
                    {
                        selectedCharacterID = characterSelectDisplay.GetThumbnail().GetInstanceID();
                    }
                    // Update Name Banner
                    if (characterSelectDisplay.GetThumbnail().GetInstanceID() == chara.picture.GetInstanceID())
                    {
                        characterSelectDisplay.SetCharacterName(chara.charaName);
                    }
                }
            }
        }

        public void IsCustomCharacterSelected()
        {
            loadCharacter = false;
            checkSelectedCharacter = false;
            foreach (CustomCharacter chara in characters)
            {
                if (selectedCharacterID == chara.picture.GetInstanceID())
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
