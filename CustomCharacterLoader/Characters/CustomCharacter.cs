using System;
using System.IO;
using System.Text.Json;
using System.Reflection;
using Flash2;
using UnityEngine;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using Object = UnityEngine.Object;
using System.Linq;

namespace CustomCharacterLoader.CharacterManager
{
    public class CustomCharacter : UnityEngine.Object
    {
        public string charaName = "";
        public string assetName = "";
        public string base_monkey = "jet";
        public int monkey_voice_id = 0;
        public int cue_id = 0;
        public string monkey_acb = "";
        public string banana_acb = "";
        public string announcer_acb = "";

        // Super Monkey Ball Objects
        public SelMgCharaItemData itemData;

        // Unity Asset Bundle Stuff
        public AssetBundle asset;
        public Sprite icon;
        public Sprite banner;
        public Sprite pause; // NOT IMPLEMENTED

        private class CharacterJsonTemplate
        {
            public string asset_bundle { get; set; }
            public string base_monkey { get; set; } = "jet";
            public string monkey_acb { get; set; } = "";
            public string banana_acb { get; set; } = "";
        }

        // Reads a json file then get the asset bundle and base_character
        public CustomCharacter(string characterName, string json, string dir)
        {
            CharacterJsonTemplate template = JsonSerializer.Deserialize<CharacterJsonTemplate>(json);

            template.base_monkey = template.base_monkey.ToLower();
            String[] charakind = Array.ConvertAll(Enum.GetNames(typeof(Chara.eKind)), chara => chara.ToString().ToLower());
            if (Array.Exists(charakind, chara => chara == template.base_monkey))
            {
                this.base_monkey = template.base_monkey;
            }

            this.charaName = characterName;
            this.assetName = template.asset_bundle;

            // Open asset bundle
            try
            {
                Main.Output("Loading Character: " + this.charaName);
                this.asset = AssetBundle.LoadFromFile(Path.Combine(dir, this.assetName));

                if (this.asset == null)
                {
                    Main.Output("Cant find Asset bundle:" + dir + this.assetName);
                }
                else
                {
                    Main.Output("Loaded Asset Bundle: " + this.assetName);
                }
            }
            catch (Exception ex)
            {
                Main.Output("Unable to load file: " + Path.Combine(dir, this.assetName));
            }

            // Open sound files
            CriAtomExAcb sounds = CriAtomExAcb.LoadAcbFile(null, Path.Combine(dir, template.monkey_acb), null);
            if(sounds != null)
            {
                this.monkey_acb =  Path.Combine(dir, template.monkey_acb);
                sounds.Dispose();
            }
        }

        // Create item data for character select screen
        public void CreateItemData(SelMgCharaItemDataListObject itemDataList)
        {
            // Get CharaKind
            SelMgCharaItemData clone = itemDataList.m_ItemDataList[0];
            foreach (SelMgCharaItemData character in itemDataList.m_ItemDataList.list)
            {
                if (character.characterKind.ToString().ToLower() == this.base_monkey)
                {
                    clone = character;
                    break;
                }
            }

            // Character Select Sprites
            this.icon = this.asset.LoadAsset<Sprite>("icon");
            this.banner = this.asset.LoadAsset<Sprite>("banner");
            this.pause = this.asset.LoadAsset<Sprite>("pause");

            // Item Data class
            this.itemData = new SelMgCharaItemData();
            this.itemData.costumeIndex = 0;
            this.itemData.m_CharacterKind = clone.m_CharacterKind;
            this.itemData.m_CostumeList = new SelMgCharaItemData.CostumeList();

            // Costume
            SelMgCharaItemData.CostumeData costume = new SelMgCharaItemData.CostumeData();
            costume.m_DisplayItemName = clone.costumeList[0].m_DisplayItemName;
            costume.m_PartsSetIndex = clone.costumeList[0].m_PartsSetIndex;
            costume.m_PointShopID = clone.costumeList[0].m_PointShopID;
            costume.spriteIcon = this.icon;
            costume._spriteIcon_k__BackingField = this.icon;
            costume.spritePicture = this.pause;
            costume._spritePicture_k__BackingField = this.pause;

            this.itemData.m_CostumeList.Add(costume);

            // Item Data Cont..
            this.itemData.m_DescriptionText = clone.m_DescriptionText;
            this.itemData.m_IsHideText = clone.m_IsHideText;

            IntPtr cueIdPtr = IL2CPP.GetIl2CppNestedType(IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "Flash2", "sound_id"), "cue");
            Il2CppSystem.Type cueIdType = UnhollowerRuntimeLib.Il2CppType.TypeFromPointer(cueIdPtr);

            Il2CppSystem.Reflection.Assembly assembly = Il2CppSystem.AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "Assembly-CSharp");
            Il2CppSystem.Type enumRuntimeHelper = assembly.GetType("Framework.EnumRuntimeHelper`1");
            Il2CppSystem.Type erhCue = enumRuntimeHelper.MakeGenericType(new Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[] { cueIdType }));

            Il2CppSystem.Reflection.MethodInfo cueValToNameGetter = erhCue.GetProperty("valueToNameCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<sound_id.cue, string> cueValToName = cueValToNameGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0)).Cast<Il2CppSystem.Collections.Generic.Dictionary<sound_id.cue, string>>();
            cueValToName.Add((sound_id.cue)this.cue_id, this.cue_id.ToString());

            Il2CppSystem.Reflection.MethodInfo cueNameToValGetter = erhCue.GetProperty("nameToValueCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<string, sound_id.cue> cueNameToVal = cueNameToValGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0)).Cast<Il2CppSystem.Collections.Generic.Dictionary<string, sound_id.cue>>();
            cueNameToVal.Add(this.cue_id.ToString(), (sound_id.cue)this.cue_id);

            this.itemData.m_NarrationCueID = this.cue_id.ToString();
            this.itemData.m_PointShopID = "Invalid";
            this.itemData.m_SupplementaryText = clone.m_SupplementaryText;
            this.itemData.m_Text = clone.m_Text;

            itemDataList.m_ItemDataList.Add(this.itemData);
        }
        public void UpdateCharacterSelect()
        {
            // Update icons
            if (!this.icon)
            {
                this.icon = this.asset.LoadAsset<Sprite>("icon");
            }
            if (!this.banner)
            {
                this.banner = this.asset.LoadAsset<Sprite>("banner");
            }
            this.itemData.costumeList[0].spritePicture = this.banner;
            this.itemData.costumeList[0].spriteIcon = this.icon;
        }

        public GameObject InitializeCharacter(Shader shader, Shader eyeShader)
        {
            GameObject modModel = Object.Instantiate(this.asset.LoadAsset<GameObject>("character"));

            // set shaders
            Il2CppArrayBase<SkinnedMeshRenderer> MeshRenderers = modModel.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer meshRenderer in MeshRenderers)
            {
                foreach (Material material in meshRenderer.materials)
                {
                    if (!material.name.Contains("balls") && !material.name.Contains("Eye") && !material.name.Contains("Custom") && !material.name.Contains("Alpha")) // if someone asks for yet another exception throw them out a window
                    {
                        material.shader = shader;
                    }
                    else if (material.name.Contains("Eye") && material.name.Contains("Alpha"))
                    {
                        material.shader = eyeShader;
                    }
                    else
                    {
                        // do nothing adachi_true :)
                    }
                }
            }
            
            modModel.SetActive(true);
            return modModel;
        }
    }
}