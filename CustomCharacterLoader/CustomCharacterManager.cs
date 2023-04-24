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
        protected bool importedCharacters = false;
        public List<CustomCharacter> characters = new List<CustomCharacter>();

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
        public void ImportCharacters(bool force=false)
        {
            if (!importedCharacters || force)
            {
                SelMgCharaItemDataListObject[] gameCharacterList = Resources.FindObjectsOfTypeAll<SelMgCharaItemDataListObject>();
                if (gameCharacterList != null && gameCharacterList.Length > 0)
                {
                    foreach (CustomCharacter chara in characters)
                    {
                        chara.CreateItemData(gameCharacterList[0]);
                        BannerID.Add(chara.icon.GetInstanceID());
                        BannerNames.Add(chara.charaName);
                    }
                    Main.Output("Created Custom Character Slots.");
                    importedCharacters = true;
                }
            }
            Update(); // make sure pictures stay
        }

        // Update Icons
        public void Update()
        {
            foreach (CustomCharacter chara in characters)
            {
                chara.UpdateCharacterSelect();
            }
        }
    }
}
