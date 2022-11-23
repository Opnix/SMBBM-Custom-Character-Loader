using System;
using System.IO;
using System.Text.Json;
using System.Reflection;
using Flash2;
using UnityEngine;
using UnhollowerBaseLib;
using Object = UnityEngine.Object;

namespace CustomCharacterLoader
{
    public class CustomCharacter : UnityEngine.Object
    {
        public string charaName = "";
        private string assetName = "";
        private string voiceBank = "";
        private bool gameShaders = false;
        private bool reskin = false;

        // Super Monkey Ball Objects
        public SelMgCharaItemData itemData;

        // Unity Asset Bundle Stuff
        public AssetBundle asset;
        public Sprite icon;
        public Sprite banner;
        public Sprite pause;

        // Sound Files
        public string monkeeAcbPath = "";
        public string bananaAcbPath = "";

        private class CharacterTemp
        {
            public string asset_bundle { get; set; }
            public string base_monkey { get; set; }
            public string monkeeAcb { get; set; }
            public string bananaAcb { get; set; }
            public bool game_shaders { get; set; }
            public bool reskin { get; set; }
        }

        // reads a json file then get the asset bundle and base_character
        public CustomCharacter(string characterName, string json, string dir)
        {
            CharacterTemp template = JsonSerializer.Deserialize<CharacterTemp>(json);
            if (template.base_monkey == "")
            {
                this.voiceBank = "Aiai";
            }
            else
            {
                this.voiceBank = template.base_monkey;
            }

            this.charaName = characterName;
            this.assetName = template.asset_bundle;
            this.gameShaders = template.game_shaders;
            this.reskin = template.reskin;

            // get asset bundle
            Main.Output("Loading Character: " + this.charaName);
            this.asset = AssetBundle.LoadFromFile(Path.Combine(dir, this.assetName));
            if (this.asset == null)
            {
                Main.Output("Cant find Asset bundle for " + dir + this.assetName);
            }
            else
            {
                Main.Output("Loaded Asset Bundle: " + this.assetName);

                // get sound files if any
                if (template.monkeeAcb != "")
                {
                    monkeeAcbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Monkeys\" + template.monkeeAcb + ".acb");
                }
                if (template.bananaAcb != "")
                {
                    bananaAcbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\" + template.bananaAcb + ".acb");
                }
            }
        }

        public void CreateItemData(SelMgCharaItemDataListObject itemDataList)
        {
            // Find the character the custom one is based on (for voice banks)
            SelMgCharaItemData clone = itemDataList.m_ItemDataList[0];
            foreach (SelMgCharaItemData character in itemDataList.m_ItemDataList.list)
            {
                if (character.characterKind.ToString().ToLower() == this.voiceBank)
                {
                    clone = character;
                    break;
                }
            }

            this.icon = this.asset.LoadAsset<Sprite>("icon");
            this.banner = this.asset.LoadAsset<Sprite>("banner");

            this.itemData = new SelMgCharaItemData();
            this.itemData.costumeIndex = 0;
            this.itemData.m_CharacterKind = clone.m_CharacterKind;
            this.itemData.m_CostumeList = new SelMgCharaItemData.CostumeList();

            SelMgCharaItemData.CostumeData costume = new SelMgCharaItemData.CostumeData();
            costume.m_DisplayItemName = clone.costumeList[0].m_DisplayItemName;
            costume.m_PartsSetIndex = clone.costumeList[0].m_PartsSetIndex;
            costume.m_PointShopID = clone.costumeList[0].m_PointShopID;
            costume.spriteIcon = this.icon;
            costume._spriteIcon_k__BackingField = this.icon;
            costume.spritePicture = this.banner;
            costume._spritePicture_k__BackingField = this.banner;
            this.itemData.m_CostumeList.Add(costume);

            this.itemData.m_DescriptionText = clone.m_DescriptionText;
            this.itemData.m_IsHideText = clone.m_IsHideText;
            this.itemData.m_NarrationCueID = clone.m_NarrationCueID;
            this.itemData.m_PointShopID = "Invalid";
            this.itemData.m_SupplementaryText = clone.m_SupplementaryText;
            this.itemData.m_Text = clone.m_Text;

            itemDataList.m_ItemDataList.Add(this.itemData);
        }

        public void UpdateCharacterSelect()
        {
            // Update Icons
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

        public GameObject InitializeCharacter(Shader shader)
        {
            GameObject modModel = Object.Instantiate(this.asset.LoadAsset<GameObject>("character"));
            Il2CppReferenceArray<Material> materials = modModel.GetComponentInChildren<SkinnedMeshRenderer>().materials;

            if (this.gameShaders)
            {
                foreach (Material material in materials)
                {
                    material.shader = shader;
                }
            }

            modModel.SetActive(true);
            return modModel;
        }
    }
}