using System;
using System.IO;
using System.Text.Json;
using Flash2;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomCharacterLoader
{
    public class CustomCharacter : UnityEngine.Object
    {
        public string charaName = "";
        private string asset_name = "";
        private string voiceBank = "";

        // Super Monkey Ball Objects
        public SelMgCharaItemData itemData;

        // Unity Asset Bundle Stuff
        public AssetBundle asset;
        public Sprite icon;
        public Sprite banner;
        public Sprite pause;

        private class CharacterTemp
        {
            public string chara_name { get; set; }
            public string voice_bank { get; set; }
        }

        // reads a json file then get the asset bundle and base_character
        public CustomCharacter(string asset_name, string json, string CHARA_PATH)
        {
            CharacterTemp template = JsonSerializer.Deserialize<CharacterTemp>(json);
            if (template.voice_bank == "")
            {
                this.voiceBank = "Aiai";
            }
            else
            {
                this.voiceBank = template.voice_bank;
            }

            this.charaName = template.chara_name;
            this.asset_name = asset_name;

            Console.WriteLine("Loading Asset Bundle: " + this.asset_name);

            // get asset bundle
            this.asset = AssetBundle.LoadFromFile(Path.Combine(CHARA_PATH, this.asset_name));
            if (this.asset == null)
            {
                Console.WriteLine("Cant find Asset bundle for " + CHARA_PATH + this.asset_name);
            }
            else
            {
                Console.WriteLine("Loaded Asset Bundle: " + this.asset_name);
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

        public GameObject InitializeCharacter(Shader shader)
        {
            GameObject modModel = Object.Instantiate(this.asset.LoadAsset<GameObject>("character"));
            modModel.GetComponentInChildren<SkinnedMeshRenderer>().material.shader = shader;
            modModel.SetActive(true);

            return modModel;
        }
    }
}