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
    public class PlayerLoader : MonoBehaviour
    {
        // loading character stuff
        public bool loadCharacter = false;
        public CustomCharacter selectedCharacter;
        private GameObject CustomMonkeyObject;

        // change character costume costume
        public bool removedCostume = false;
        public CharaCustomizeManager charaCustomizeManager = null; // manages ingame costumes
        private CharaCustomize.PartsSet partSet;
        private CharaCustomize.PartsKeyDict costumeParts;

        public PlayerLoader() { }
        public PlayerLoader(IntPtr ptr) : base(ptr) { }

        // check if a custom model needs to be loaded
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
        public void LoadPlayer()
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
