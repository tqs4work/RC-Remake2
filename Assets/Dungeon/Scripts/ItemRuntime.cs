using System;
using UnityEngine;

[System.Serializable]
public class ItemRuntime
{
    public ItemType itemType;

    public string itemID;
    public string itemName;
    public string description;    

    

    [Header("H�nh ?nh")]
    public Sprite icon;

    [Header("Ch? s? v?t ph?m")]
    public bool isStackable;
    public int maxStack;
    public int price;

    [Header("V?t ph?m ti�u th?")]
    public int quantity;
    public int hpAmount;
    public int mpAmount;

    [Header("V?t ph?m chi?n ??u")]
    public float atk;
    public float def;
    public float durability = 100f;

    [Header("H?t gi?ng")]
    public float growTime;

    [Header("Ch? s? ??c bi?t")]
    public float bonus;
    public int upgradeLevel; // 0-5

    [Header("Upgrade Icons")]
    
    public Item itemData;   // reference tới ScriptableObject gốc

    public static ItemRuntime FromItem(Item item)
    {
        return new ItemRuntime
        {
            itemData = item,
            itemType = item.itemType,

            itemID = item.itemID,
            itemName = item.itemName,
            description = item.description,
            icon = item.icon,

            isStackable = item.isStackable,
            maxStack = item.maxStack,
            
            price = item.price,


            quantity = item.quantity,
            hpAmount = item.hpAmount,
            mpAmount = item.mpAmount,

            atk = item.atk,
            def = item.def,
            durability = item.maxDurability,

            growTime = item.growTime,

            bonus = item.bonus,
            upgradeLevel = 0
        };
    }

    public static ItemRuntime FromData(Item2String data)
    {
        string baseID = data.itemID.Split(' ')[0];
        Item baseItem = ItemDatabase.Instance.GetItemByID(baseID);        

        return new ItemRuntime
        {
            itemData = baseItem, 
            itemType = Enum.Parse<ItemType>(data.itemType),
            itemID = data.itemID,
            itemName = data.itemName,
            description = data.description,
            icon = Resources.Load<Sprite>(data.icon),

            isStackable = bool.Parse(data.isStackable),
            maxStack = int.Parse(data.maxStack),

            
            price = int.Parse(data.price),


            quantity = int.Parse(data.quantity),
            hpAmount = int.Parse(data.hpAmount),
            mpAmount = int.Parse(data.mpAmount),

            atk = float.Parse(data.atk),
            def = float.Parse(data.def),
            durability = float.Parse(data.durability),
            upgradeLevel = int.Parse(data.upgradeLevel),

            growTime = float.Parse(data.growTime),

            bonus = float.Parse(data.bonus)
        };
    }
    public void ApplyUpgradeVisual()
    {
       upgradeLevel = Mathf.Clamp(upgradeLevel, 0, 5);

        if (itemData != null &&
            itemData.upgradeIcons != null &&
            itemData.upgradeIcons.Length > upgradeLevel)
        {
            icon = itemData.upgradeIcons[upgradeLevel];
        }
    }
}

