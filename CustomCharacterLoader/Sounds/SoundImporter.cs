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

namespace CustomCharacterLoader.Sounds
{
    public class SoundImporter : MonoBehaviour
    {
        private int sound_id = 6000;
        private int cue_id = 10000;
        private Sound sound = null;
        public bool importedSounds = false;
        public SoundImporter() { }
        public SoundImporter(IntPtr ptr) : base(ptr) { }

        public void Load()
        {
            if(!this.importedSounds)
            {
                if (this.sound == null)
                {
                    Console.WriteLine("Finding sound instance");
                    this.sound = Sound.Instance;
                }
                else
                {
                    Console.WriteLine("Loading sounds");
                    foreach (CustomCharacter character in Main.characterManager.characters)
                    {
                        if (character.monkey_acb != null)
                        {
                            Sound.Instance.m_cueSheetParamDict[(sound_id.cuesheet)sound_id] = new Sound.cuesheet_param_t(sound_id.ToString(), character.monkey_acb, null); //monkey_acb = full file path
                            Sound.Instance.m_cueParamDict[(sound_id.cue)cue_id] = new Sound.cue_param_t((sound_id.cuesheet)sound_id, "start");
                            Sound.Instance.LoadCueSheetASync((sound_id.cuesheet)sound_id);
                            character.cue_id = cue_id;
                            character.monkey_voice_id = sound_id;
                            sound_id++;
                            cue_id++;
                        }
                    }
                    this.importedSounds = true;
                }
            }
        }
    }
}
