using System.Collections.Generic;
using Flash2;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System;
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using System.Reflection;
using System.Linq;

namespace CustomCharacterLoader
{
    class MonkeyVoices
    {
        public static bool load = false;

        public MonkeyVoices()
        {
            Main.Output("Guest Characters Voice Pack Found! I CAN FINALLY TALK!!!!!!");
        }

        public void LoadSounds(CustomCharacter chara)
        {
            if (load)
            {
                // replace the Guest Voice Controller
                if(Guest.Main._dynamicRoll != null)
                {
                    Player player = Object.FindObjectOfType<Player>();
                    Object.Destroy(Guest.Main._dynamicRoll);
                    Guest.Main._dynamicRoll = new CustomVoiceController(player.GetCameraController().gameObject.AddComponent(Il2CppType.Of<CustomVoiceController>()).Pointer);
                    load = false;
                }
            }
        }
    }

    internal class CustomVoiceController : Guest.GuestCharacters
    {

        public CustomVoiceController(IntPtr value) : base(value) { }

        public new void Awake()
        {
            _player = FindObjectOfType<Player>();

            _monkee = new Guest.Monkee();
            _banana = new Guest.Banana();

            _boundArray = new List<float>();
            _collideArray = new List<float>();
            _dropArray = new List<float>();
            _softArray = new List<float>();

            _monkeeArray = new string[] { "aiai", "baby", "doctor", "gongon", "jam", "jet", "meemee", "yanyan" };
            _guestArray = new string[] { "beat", "kiryu", "sonic", "tails", "dlc01", "dlc02", "dlc03" };
            _consoleArray = new string[] { "dreamcast", "gamegear", "segasaturn" };

            _monkeeType = _player.charaKind.ToString().ToLower();

            // Create players for each sfx
            _bananaPlayer = new CriAtomExPlayer();
            _monkeePlayer = new CriAtomExPlayer();

            _timer = MainGame.mainGameStage.m_GameTimer;

            _isStart = false;
            _isFallout = false;
            _isGoal = false;
            _isGoalFly = false;
            _isBumped = false;
        }

        private void Start()
        {
            bool monkeeVoiceBool = true;
            bool bananaVoiceBool = true;
            if (Main.customCharacterManager.selectedCharacter.monkeeAcbPath != "")
            {
                Main.Output(Main.customCharacterManager.selectedCharacter.monkeeAcbPath);
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, Main.customCharacterManager.selectedCharacter.monkeeAcbPath, null);
                if (_monkeeAcb != null)
                {
                    monkeeVoiceBool = false;
                }
            }
            if (Main.customCharacterManager.selectedCharacter.bananaAcbPath != "")
            {
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, Main.customCharacterManager.selectedCharacter.bananaAcbPath, null);
                if (_bananaAcb != null)
                {
                    bananaVoiceBool = false;
                }
            }

            if (_monkeeArray.Contains(_monkeeType))
            {
                if (monkeeVoiceBool)
                {
                    string monkeePath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Monkeys\vo_" + _monkeeType + ".acb");
                    _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);
                }

                if (bananaVoiceBool)
                {
                    string bananaPath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Bananas\bananas.acb");
                    _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
                }
            }
            else if (_consoleArray.Contains(_monkeeType))
            {
                if (monkeeVoiceBool)
                {
                    string monkeePath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Consoles\vo_" + _monkeeType + ".acb");
                    _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);
                }

                if (bananaVoiceBool)
                {
                    string bananaPath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Bananas\bananas.acb");
                    _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
                }
            }
            else if (_guestArray.Contains(_monkeeType))
            {
                if (monkeeVoiceBool)
                {
                    string monkeePath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Guests\vo_" + _monkeeType + ".acb");
                    _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);
                }

                if (bananaVoiceBool)
                {
                    string bananaPath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Bananas\bananas_" + _monkeeType + ".acb");
                    _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
                }
            }
            else
            {
                if (monkeeVoiceBool)
                {
                    string monkeePath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\DLC\vo_muted.acb");
                    _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);
                }
                if (bananaVoiceBool)
                {
                    string bananaPath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Bananas\bananas_muted.acb");
                    _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
                }
            }
        }
    }
}
