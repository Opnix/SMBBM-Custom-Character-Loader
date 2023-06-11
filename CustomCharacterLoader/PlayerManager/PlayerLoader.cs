using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash2;
using UnityEngine;
using UnhollowerRuntimeLib;
using Object = UnityEngine.Object;
using CustomCharacterLoader.CharacterManager;

namespace CustomCharacterLoader.PlayerManager
{
    public class PlayerLoader : MonoBehaviour
    {
        public enum CharacterType
        {
            None = 0,
            Character = 1,
            Costume = 2,
        }

        public CharacterType playerType = CharacterType.None;
        public int playerIndex;

        // Loading character stuff
        public CustomCharacter selectedCharacter;
        private GameObject customPlayerObject;

        // Change character costume costume
        private bool removedCosmetics = false;
        private CharaCustomizeManager charaCosmeticManager = null; // Manages ingame costumes
        private CharaCustomize.PartsSet selectedPartSet;
        private CharaCustomize.PartsKeyDict originalCosmeticsParts;

        public PlayerLoader() { }
        public PlayerLoader(IntPtr ptr) : base(ptr) { }

        public void Load()
        {
            // Get cosmetic manager 
            if (charaCosmeticManager == null)
            {
                CharaCustomizeManager[] charaCustomizeContainer = Resources.FindObjectsOfTypeAll<CharaCustomizeManager>();
                if (charaCustomizeContainer != null && charaCustomizeContainer.Length > 0)
                {
                    charaCosmeticManager = charaCustomizeContainer[0];
                }
            }
            // Revert changes if needed
            RevertChanges();
        }

        // Remove cosmetics from default character so they don't load over custom models.
        private void RemoveCosmetics()
        {
            Chara.eKind selectedCharaKind = selectedCharacter.itemData.characterKind;
            if (charaCosmeticManager.m_PartsSetDict.CollectionInstance.ContainsKey(selectedCharaKind))
            {
                selectedPartSet = charaCosmeticManager.m_PartsSetDict.CollectionInstance[selectedCharaKind][1];
                originalCosmeticsParts = selectedPartSet.m_PartsKeyDict;

                CharaCustomize.PartsKeyDict ballOnly = new CharaCustomize.PartsKeyDict();
                if (selectedPartSet.m_PartsKeyDict.ContainsKey(CharaCustomize.eAssignPos.Ball))
                {
                    ballOnly.Add(CharaCustomize.eAssignPos.Ball, originalCosmeticsParts[CharaCustomize.eAssignPos.Ball]);
                }

                selectedPartSet.m_PartsKeyDict = ballOnly;
                removedCosmetics = true;
            }
        }

        // Revert RemoveCosmetics() changes
        public void RevertChanges()
        {
            // Restore old costume for base character
            if (removedCosmetics)
            {
                selectedPartSet.m_PartsKeyDict = originalCosmeticsParts;
                removedCosmetics = false;
                Main.Output("restored cosmetics");
            }
        }

        // Load character according to player type
        public void CheckPlayerType()
        {
            if (playerType == CharacterType.Character)
            {
                selectedCharacter = Main.customCharacterManager.characters[playerIndex];
                Main.Output("CHARACTER!");
            }
        }

        // Insert character into the game
        public void LoadPlayer()
        {
            if (playerType == CharacterType.Character)
            {
                // Look for player object
                if (customPlayerObject == null)
                {
                    Main.soundController.load = true;
                    Main.soundController.load2 = true;
                    RemoveCosmetics();
                    customPlayerObject = CustomCharacterManager.ReplaceModel(selectedCharacter);
                }
            }
        }
    }
}
