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

namespace CustomCharacterLoader
{
    public class CustomCharacterManager : MonoBehaviour
    {
        // name titles for custom characters
        public static List<int> BannerID = new List<int>();
        public static List<String> BannerNames = new List<String>();

        // importing stuff
        public bool importedCharacters = false;
        public List<CustomCharacter> characters = new List<CustomCharacter>();

        // loading character stuff
        public bool loadCharacter = false;
        public CustomCharacter selectedCharacter;
        private GameObject CustomMonkeyObject;

        // removing Aiai costume
        public bool removedCostume = false;
        public CharaCustomizeManager charaCustomizeManager = null;
        private CharaCustomize.PartsSet partSet;
        private CharaCustomize.PartsKeyDict costumeParts;

        public CustomCharacterManager() { }
        public CustomCharacterManager(IntPtr ptr) : base(ptr) { }
        public CustomCharacterManager(IntPtr ptr, string PATH) : base(ptr)
        {
            // go to each folder in Character folder
            foreach (string dir in Directory.GetDirectories(PATH))
            {
                // Reads all the Json files in folder
                foreach (string json in Directory.GetFiles(dir, "*.json"))
                {
                    StreamReader reader = new StreamReader(json);
                    string data = reader.ReadToEnd();

                    string name = dir.Substring(dir.LastIndexOf("\\") + 1);

                    CustomCharacter character = new CustomCharacter(name, data, dir);

                    if (character.asset != null)
                    {
                        characters.Add(character);
                    }
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
            Main.Output("Created Custom Character Slots.");
            importedCharacters = true;
        }

        // Update Icons
        public void Update()
        {
            foreach (CustomCharacter chara in characters)
            {
                chara.UpdateCharacterSelect();
            }
        }

        // Check if a custom character is selected
        public void GetCustomCharacterSelected()
        {
            loadCharacter = false;
            Patches.MgCharaOnSubmitPatch.isCustomCharacter = false;
            Patches.TaCharaOnSubmitPatch.isCustomCharacter = false;

            foreach (CustomCharacter chara in characters)
            {
                if (Patches.MgCharaOnSubmitPatch.selectedCharacterID == chara.icon.GetInstanceID())
                {
                    selectedCharacter = chara;
                    loadCharacter = true;

                    // Prep to remove costume parts if needed
                    if (charaCustomizeManager.m_PartsSetDict.CollectionInstance.ContainsKey(chara.itemData.characterKind))
                    {
                        partSet = charaCustomizeManager.m_PartsSetDict.CollectionInstance[chara.itemData.characterKind][1];
                        costumeParts = partSet.m_PartsKeyDict;

                        CharaCustomize.PartsKeyDict ballOnly = new CharaCustomize.PartsKeyDict();
                        if (partSet.m_PartsKeyDict.ContainsKey(CharaCustomize.eAssignPos.Ball))
                        {
                            ballOnly.Add(CharaCustomize.eAssignPos.Ball, costumeParts[CharaCustomize.eAssignPos.Ball]);
                        }

                        partSet.m_PartsKeyDict = ballOnly;
                        removedCostume = true;
                    }
                    break;
                }
            }
        }

        // revert any changes when not in main game
        public void RevertChanges()
        {
            // restore old costume for base character
            if(removedCostume)
            {
                partSet.m_PartsKeyDict = costumeParts;
                removedCostume = false;
                Main.Output("restored costume");
            }
        }

        // insert character into the game
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
                    MonkeyVoices.load = true;
                }
            }
        }
    }
}
