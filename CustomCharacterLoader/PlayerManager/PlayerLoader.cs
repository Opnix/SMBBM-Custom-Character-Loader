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
using UnhollowerBaseLib;
using Il2CppSystem.Reflection;
using BananaModManager.Shared;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using Framework.UI;

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

        public PlayerLoader() { }
        public PlayerLoader(IntPtr ptr) : base(ptr) { }

        // Load character according to player type
        public void CheckPlayerType()
        {
            if (playerType == CharacterType.Character)
            {
                selectedCharacter = Main.customCharacterManager.characters[playerIndex];
                Main.Output("Selected a custom character!");
                Main.loadCharacter = true;
            }
            else
            {
                Main.loadCharacter = false;
            }
        }

        // Insert character into the game
        public void LoadPlayer(AssetBundle AssetBundle)
        {
            if (playerType == CharacterType.Character)
            {
                // Look for player object
                if (customPlayerObject == null)
                {
                    customPlayerObject = ReplaceModel(selectedCharacter, AssetBundle);
				}
            }
        }

        public Sprite LoadPause(AssetBundle AssetBundle)
        {
            return AssetBundle.LoadAsset<Sprite>("pause");
        }

        // Replace ingame model
        public static GameObject ReplaceModel(CustomCharacter selectedCharacter, AssetBundle assetbundle)
        {
            GameObject customPlayerObject = null;
            Il2CppArrayBase<Player> players = Resources.FindObjectsOfTypeAll<Player>(); // FUTURE ME, THIS DONT WORK WITH MULTIPLE PLAYERS
            if (players.Length > 1)
            {
                GameObject playerContainer = players[0].m_Monkey.gameObject;
                GameObject target_model = playerContainer.GetComponentInChildren<Animator>().gameObject;
                Shader shader = assetbundle.LoadAsset<Shader>("assets/eatass2.shader");
				Shader eyeShader = assetbundle.LoadAsset<Shader>("assets/eyeass.shader");

				// make base character model invisible
				foreach (var component in playerContainer.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    component.enabled = false;
                }
                foreach (var component in playerContainer.GetComponentsInChildren<MeshRenderer>())
                {
                    component.enabled = false;
                }

                // load custom character
                customPlayerObject = selectedCharacter.InitializeCharacter(shader, eyeShader);
                Quaternion rotation = playerContainer.transform.rotation;
                playerContainer.transform.rotation = Quaternion.Euler(0, 0, 0);
                customPlayerObject.transform.parent = target_model.transform;
                Animator customAnimator = customPlayerObject.GetComponentInChildren<Animator>();
                playerContainer.GetComponent<MonkeyRef>().m_Animator = customAnimator;
                playerContainer.GetComponent<Monkey>().m_Animator = customAnimator;
                playerContainer.transform.rotation = rotation;
			}
            return customPlayerObject;
        }
	}
}
