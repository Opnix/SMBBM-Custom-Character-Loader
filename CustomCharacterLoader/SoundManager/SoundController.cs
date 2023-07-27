using System.Collections.Generic;
using Flash2;
using Object = UnityEngine.Object;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCharacterLoader.SoundManager
{
    public class SoundController : MonoBehaviour
    {
        public bool load = false;
        public bool load2 = false;
        public SoundController() { }
        public SoundController(IntPtr ptr) : base(ptr) { }
        public void LoadSounds()
        {
            var player = Object.FindObjectOfType<Player>();

            // Clear what we did if player is null
            if (player == null)
            {
                Object.Destroy(Guest.Main._dynamicRoll);
                Object.Destroy(DynamicSounds.Main._dynamicRoll);
                Guest.Main._dynamicRoll = null;
                DynamicSounds.Main._dynamicRoll = null;
                return;
            }

            // Replace with our sound controller
            if (load & Guest.Main._dynamicRoll != null)
            {
                Object.Destroy(Guest.Main._dynamicRoll);
                Guest.Main._dynamicRoll = new UninvitedGuests(player.GetCameraController().gameObject.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UninvitedGuests>()).Pointer);
                load = false;
            }

            // Replace DynamicsSounds player sounds with dummy controller
            if (load2 & DynamicSounds.Main._dynamicRoll != null)
            {
                Object.Destroy(DynamicSounds.Main._dynamicRoll);
                DynamicSounds.Main._dynamicRoll = new DummyController(player.GetCameraController().gameObject.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<DummyController>()).Pointer);
                load2 = false;
            }
        }
    }
}
