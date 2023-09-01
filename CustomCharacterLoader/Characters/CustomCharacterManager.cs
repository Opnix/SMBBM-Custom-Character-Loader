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

namespace CustomCharacterLoader.CharacterManager
{
    public class CustomCharacterManager : MonoBehaviour
    {
        // Lists of character stuff
        public List<CustomCharacter> characters = new List<CustomCharacter>();

        public CustomCharacterManager() { }
        public CustomCharacterManager(IntPtr ptr) : base(ptr) { }
        public CustomCharacterManager(IntPtr ptr, string path) : base(ptr)
        {
            // Read the Characters folder
            foreach (string dir in Directory.GetDirectories(path))
            {
                Console.WriteLine(dir);
                // Reads all the Json files in folders
                foreach (string json in Directory.GetFiles(dir, "*.json"))
                {
                    StreamReader reader = new StreamReader(json);
                    string data = reader.ReadToEnd();
                    string name = dir.Substring(dir.LastIndexOf("\\") + 1);
                    CustomCharacter character = new CustomCharacter(name, data, dir);
                    reader.Close();

                    if (character.asset != null)
                    {
                        characters.Add(character);
                    }
                }
            }
        }

        // Create character select icons for all custom characters
        public static bool importedCharacters = false;
        public void Load()
        {
            if (!importedCharacters)
            {
                SelMgCharaItemDataListObject[] gameCharacterList = Resources.FindObjectsOfTypeAll<SelMgCharaItemDataListObject>(); // character select page
                if (gameCharacterList != null && gameCharacterList.Length > 0)
                {
                    foreach (CustomCharacter chara in characters)
                    {
                        chara.CreateItemData(gameCharacterList[0]);
                    }
                    Main.Output("Created Custom Character Slots.");
                    importedCharacters = true;
                }
            }
            Update(); // Make sure pictures stay
        }

        // Update icons
        public void Update()
        {
            foreach (CustomCharacter chara in characters)
            {
                chara.UpdateCharacterSelect();
            }
        }
    }
}
