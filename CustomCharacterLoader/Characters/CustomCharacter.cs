using System;
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
        public string assetName = "";
        public string base_monkey = "Aiai";
        public string monkeeAcbPath = "";
        public string bananaAcbPath = "";
        public bool shader = true;

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
            public string base_monkey { get; set; }
            public string monkeeAcb { get; set; }
            public string bananaAcb { get; set; }
            public bool shader { get; set; } = true;
        }

        // Reads a json file then get the asset bundle and base_character
        public CustomCharacter(string characterName, string json, string dir)
        {
            // get base monkey
            CharacterJsonTemplate template = JsonSerializer.Deserialize<CharacterJsonTemplate>(json);
            if(template.base_monkey != null)
            {
                if (Enum.IsDefined(typeof(Chara.eKind), template.base_monkey))
                {
                    this.base_monkey = template.base_monkey;
                }
            }

            this.charaName = characterName;
            this.assetName = template.asset_bundle;
            this.shader = template.shader;

            // Open asset bundle
            try
            {
                Main.Output("Loading Character: " + this.charaName);
                this.asset = AssetBundle.LoadFromFile(Path.Combine(dir, this.assetName));
            }
            catch (Exception ex)
            {
                Main.Output("Unable to load file: " + Path.Combine(dir, this.assetName));
            }

            // Check if asset bundle loaded
            if (this.asset == null)
            {
                Main.Output("Cant find Asset bundle:" + dir + this.assetName);
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
                if (character.characterKind.ToString().ToLower() == this.base_monkey)
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

        public GameObject InitializeCharacter(Shader shader, Shader eyeShader)
        {
            GameObject modModel = Object.Instantiate(this.asset.LoadAsset<GameObject>("character"));

            // set shaders
            if (this.shader == true)
            {
                Il2CppArrayBase<SkinnedMeshRenderer> MeshRenderers = modModel.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer meshRenderer in MeshRenderers)
                {
                    foreach (Material material in meshRenderer.materials)
                    {
                        if (!material.name.Contains("balls") && !material.name.Contains("Eye"))
                        {
                            material.shader = shader;
                        }
                        else if (material.name.Contains("Eye"))
                        {
                            material.shader = eyeShader;
                        }
                        else
                        {
                            // do nothing adachi_true :)
                        }
                    }
                }
            }
            modModel.SetActive(true);
            return modModel;
        }
    }
}