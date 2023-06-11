using System.Collections.Generic;
using Flash2;
using Object = UnityEngine.Object;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCharacterLoader.SoundManager
{
    public class DummyController : DynamicSounds.SoundController
    {   
        public DummyController(IntPtr value) : base(value) { }
        public new void Awake()
        {
            base.Awake();
            _monkeeType = "NA";
        }
    }

    // Guest Characters Sound Controller modified so every monkey is in the guest[] and some additional code to mix and match for custom character sounds.
    public class UninvitedGuests : Guest.GuestCharacters
    {
        public string[] _realGuestArray;
        public UninvitedGuests(IntPtr value) : base(value) { }

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
            _realGuestArray = new string[] { "beat", "kiryu", "sonic", "tails", "dlc01", "dlc02", "dlc03" };
            _consoleArray = new string[] { "dreamcast", "gamegear", "segasaturn" };
            _dlcArray = new string[] { "suezo", "hellokitty", "morgana" };
            _guestArray = new string[] { "aiai", "baby", "doctor", "gongon", "jam", "jet", "meemee", "yanyan", "beat", "kiryu", "sonic", "tails", "dlc01", "dlc02", "dlc03", "dreamcast", "gamegear", "segasaturn", "suezo", "hellokitty", "morgana" };
            //_guestArray = _realGuestArray;

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
            // check if there's custom voice packs
            bool monkeeVoiceBool = true;
            bool bananaVoiceBool = true;
            if (Main.playerLoader.playerType != PlayerManager.PlayerLoader.CharacterType.None)
            {
                if (Main.playerLoader.selectedCharacter.monkeeAcbPath != "")
                {
                    _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, Main.playerLoader.selectedCharacter.monkeeAcbPath, null);
                    if (_monkeeAcb != null)
                    {
                        monkeeVoiceBool = false;
                    }
                }
                if (Main.playerLoader.selectedCharacter.bananaAcbPath != "")
                {
                    _bananaAcb = CriAtomExAcb.LoadAcbFile(null, Main.playerLoader.selectedCharacter.bananaAcbPath, null);
                    if (_bananaAcb != null)
                    {
                        bananaVoiceBool = false;
                    }
                }
            }

            // assign any missing voice packs to default
            string monkeePath = "";
            string bananaPath = "";
            if (_monkeeArray.Contains(_monkeeType))
            {
                if (monkeeVoiceBool) { monkeePath = Path.Combine(Main.DYNAMIC_SOUNDS_PATH, @"Sounds\Monkeys\vo_" + _monkeeType + ".acb"); }
                if (bananaVoiceBool) { bananaPath = Path.Combine(Main.DYNAMIC_SOUNDS_PATH, @"Sounds\Bananas\bananas.acb"); }
            }
            else if (_consoleArray.Contains(_monkeeType))
            {
                if (monkeeVoiceBool) { monkeePath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Consoles\vo_" + _monkeeType + ".acb"); }
                if (bananaVoiceBool) { bananaPath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Bananas\bananas.acb"); }
            }
            else if (_realGuestArray.Contains(_monkeeType))
            {
                if (monkeeVoiceBool) { monkeePath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Guests\vo_" + _monkeeType + ".acb"); }
                if (bananaVoiceBool) { bananaPath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Bananas\bananas_" + _monkeeType + ".acb"); }
            }
            else if (_dlcArray.Contains(_monkeeType))
            {
                if (monkeeVoiceBool) { monkeePath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\DLC\vo_" + _monkeeType + ".acb"); }
                if (bananaVoiceBool) { bananaPath = Path.Combine(Main.GUEST_CHARACTER_PATH, @"Sounds\Bananas\bananas_" + _monkeeType + ".acb"); }
            }
            else
            {
                if (monkeeVoiceBool) { monkeePath = Path.Combine(Main.DYNAMIC_SOUNDS_PATH, @"Sounds\DLC\vo_muted.acb"); }
                else { monkeePath = Path.Combine(Main.DYNAMIC_SOUNDS_PATH, @"Sounds\DLC\vo_muted.acb"); }

                if (bananaVoiceBool) { bananaPath = Path.Combine(Main.DYNAMIC_SOUNDS_PATH, @"Sounds\Bananas\bananas_muted.acb"); }
                else { bananaPath = Path.Combine(Main.DYNAMIC_SOUNDS_PATH, @"Sounds\Bananas\bananas_muted.acb"); }
            }
            _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);
            _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
        }
    }
}
