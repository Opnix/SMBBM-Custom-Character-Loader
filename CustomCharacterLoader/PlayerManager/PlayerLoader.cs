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
        private static GameObject pauseChara = null;
        private static Image imageComponent = null;

        public PlayerLoader() { }
        public PlayerLoader(IntPtr ptr) : base(ptr) { }

        // Load character according to player type
        public void CheckPlayerType()
        {
            if (playerType == CharacterType.Character)
            {
                selectedCharacter = Main.characterManager.characters[playerIndex];
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

                    //ReplaceBanana(selectedCharacter, AssetBundle);
                }

                // Change Pause Icon
                if (selectedCharacter.pause == null)
                {
                    selectedCharacter.pause = selectedCharacter.asset.LoadAsset<Sprite>("pause");
                }
                if (pauseChara != null)
                {
                    if (imageComponent != null)
                    {
                        imageComponent.sprite = selectedCharacter.pause;
                    }
                    else
                    {
                        imageComponent = pauseChara.transform.GetChild(0).GetComponent<Image>();
                    }
                }
                else
                {
                    pauseChara = GameObject.Find("pos_pause_chara");
                }
            }
        }

        // Replace ingame model
        public static GameObject ReplaceModel(CustomCharacter selectedCharacter, AssetBundle assetbundle)
        {
            GameObject customPlayerObject = null;
            Il2CppArrayBase<Player> players = Resources.FindObjectsOfTypeAll<Player>(); // FUTURE, THIS DONT WORK WITH MULTIPLE PLAYERS
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


// Replace Banana model
/*
public static void ReplaceBanana(CustomCharacter selectedCharacter, AssetBundle assetbundle)
{
    Il2CppArrayBase<Banana> bananas = Resources.FindObjectsOfTypeAll<Banana>();
    if (bananas.Length > 1)
    {
        Shader shader = assetbundle.LoadAsset<Shader>("assets/eatass2.shader");

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
}
*/