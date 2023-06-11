﻿using System;
using System.IO;
using System.Text.Json;
using System.Reflection;
using Flash2;
using UnityEngine;
using UnhollowerBaseLib;
using Object = UnityEngine.Object;

namespace CustomCharacterLoader.CharacterManager
{
    public class CustomCharacter : UnityEngine.Object
    {
        public string charaName = "";
        private string assetName = "";
        private string voiceBank = "";
        private bool gameShaders = false;

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
        }

        // Reads a json file then get the asset bundle and base_character
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

            // Get asset bundle
            Main.Output("Loading Character: " + this.charaName);

            try
            {
                this.asset = AssetBundle.LoadFromFile(Path.Combine(dir, this.assetName));
            }
            catch (Exception ex)
            {
                Main.Output("Unable to load file: " + Path.Combine(dir, this.assetName));
            }

            if (this.asset == null)
            {
                Main.Output("Cant find Asset bundle for " + dir + this.assetName);
            }
            else
            {
                Main.Output("Loaded Asset Bundle: " + this.assetName);

                // Get sound files if any
                if (template.monkeeAcb != "")
                {
                    monkeeAcbPath = Path.Combine(Main.PATH, @"Sounds\Monkeys\" + template.monkeeAcb + ".acb");
                }
                if (template.bananaAcb != "")
                {
                    bananaAcbPath = Path.Combine(Main.PATH, @"Sounds\Bananas\" + template.bananaAcb + ".acb");
                }
            }
        }

        // Create item data for character select screen
        public void CreateItemData(SelMgCharaItemDataListObject itemDataList)
        {
            // Get CharaKind
            SelMgCharaItemData clone = itemDataList.m_ItemDataList[0];
            foreach (SelMgCharaItemData character in itemDataList.m_ItemDataList.list)
            {
                if (character.characterKind.ToString().ToLower() == this.voiceBank)
                {
                    clone = character;
                    break;
                }
            }

            // Character Select Sprites
            this.icon = this.asset.LoadAsset<Sprite>("icon");
            this.banner = this.asset.LoadAsset<Sprite>("banner");

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
            costume.spritePicture = this.banner;
            costume._spritePicture_k__BackingField = this.banner;
            this.itemData.m_CostumeList.Add(costume);

            // Item Data Cont..
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