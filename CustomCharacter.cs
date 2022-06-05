using System;
using System.IO;
using Flash2;
using UnityEngine;
using Object = UnityEngine.Object;


public class CustomCharacter : UnityEngine.Object
{
    // From Json
    public string chara_name = "";
    private string asset_bundle = "";
    private string voice_bank = "";

    // Super Monkey Ball Objects
    public SelMgCharaItemData itemData;

    // Unity Asset Bundle Stuff
    public AssetBundle asset;
    public Sprite icon;
    public Sprite picture;
    public Sprite pause;

    // reads a single json file then gets the asset bundle and base_character
    public CustomCharacter(string json)
	{
        voice_bank = "Aiai";

        // manually parse json since Jsonutility doesn't want to work for me
        json = json.Replace("{", String.Empty);
        json = json.Replace("}", String.Empty);
        json = json.Replace("\"", String.Empty);
        json = json.Replace("\t", String.Empty);
        json = json.Substring(2, json.Length - 3); //remove new lines from the { } lines
        foreach (string line in json.Split('\n'))
        {
            String[] keyValue = line.Split(':');
            keyValue[0] = keyValue[0].Trim();
            keyValue[1] = keyValue[1].Trim();
            if (keyValue[1].EndsWith(","))
            {
                keyValue[1] = keyValue[1].Remove(keyValue[1].Length - 1);
            }

            switch (keyValue[0])
            {
                case "name":
                    this.chara_name = keyValue[1];
                    break;
                case "asset_bundle":
                    this.asset_bundle = keyValue[1];
                    break;
                case "voice_bank":
                    this.voice_bank = keyValue[1].ToLower();
                    break;
            }
        }

        // get asset and some pics
        this.asset = AssetBundle.LoadFromFile(Path.Combine(CustomCharacterLoader.Main.PATH, this.asset_bundle));
        if (this.asset != null)
        {
        }
        else
        {
            // cant find asset
        }
    }

    public void CreateItemData(SelMgCharaItemDataListObject itemDataList)
    {
        // Find the character the custom one is based on (for voice banks)
        SelMgCharaItemData clone = itemDataList.m_ItemDataList[0];
        foreach (SelMgCharaItemData character in itemDataList.m_ItemDataList.list)
        {
            if (character.characterKind.ToString().ToLower() == this.voice_bank)
            {
                clone = character;
                break;
            }
        }

        this.itemData = new SelMgCharaItemData();
        this.itemData.costumeIndex = 0;
        this.itemData.m_CharacterKind = clone.m_CharacterKind;
        this.itemData.m_CostumeList = new SelMgCharaItemData.CostumeList();

        Console.WriteLine(clone.costumeList[0].m_DisplayItemName.key);
        Console.WriteLine(clone.costumeList[0].m_DisplayItemName.m_Key);

        SelMgCharaItemData.CostumeData costume = new SelMgCharaItemData.CostumeData();
        //costume.m_DlcDisplayItemNameKey = "mod";
        costume.m_DisplayItemName = clone.costumeList[0].m_DisplayItemName;
        costume.m_PartsSetIndex = clone.costumeList[0].m_PartsSetIndex;
        costume.m_PointShopID = clone.costumeList[0].m_PointShopID;
        costume.spriteIcon = this.icon;
        costume.spritePicture = clone.costumeList[0].spritePicture;
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
