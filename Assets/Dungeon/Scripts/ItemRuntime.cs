using UnityEngine;

[System.Serializable]
public class ItemRuntime
{
    public ItemType itemType;

    public string itemID;
    public string itemName;
    public string description;    

    

    [Header("Hình ?nh")]
    public Sprite icon;

    [Header("Ch? s? v?t ph?m")]
    public bool isStackable;
    public int maxStack;
    public int price;
    public int level;

    [Header("V?t ph?m tiêu th?")]
    public int quantity;
    public int hpAmount;
    public int mpAmount;

    [Header("V?t ph?m chi?n ??u")]
    public float atk;
    public float def;
    public float durability;

    [Header("H?t gi?ng")]
    public float growTime;

    [Header("Ch? s? ??c bi?t")]
    public float bonus;

    public static ItemRuntime FromItem(Item item)
    {
        return new ItemRuntime
        {
            itemType = item.itemType,

            itemID = item.itemID,
            itemName = item.itemName,
            description = item.description,
            icon = item.icon,

            isStackable = item.isStackable,
            maxStack = item.maxStack,
            
            price = item.price,
            level = item.level,

            quantity = item.quantity,
            hpAmount = item.hpAmount,
            mpAmount = item.mpAmount,

            atk = item.atk,
            def = item.def,
            durability = item.durability,

            growTime = item.growTime,

            bonus = item.bonus
        };
    }

    public static ItemRuntime FromData(Item2String data)
    {
        return new ItemRuntime
        {
            itemID = data.itemID,
            itemName = data.itemName,
            description = data.description,
            icon = Resources.Load<Sprite>(data.icon),

            isStackable = bool.Parse(data.isStackable),
            maxStack = int.Parse(data.maxStack),

            
            price = int.Parse(data.price),
            level = int.Parse(data.level),

            quantity = int.Parse(data.quantity),
            hpAmount = int.Parse(data.hpAmount),
            mpAmount = int.Parse(data.mpAmount),

            atk = float.Parse(data.atk),
            def = float.Parse(data.def),
            durability = float.Parse(data.durability),

            growTime = float.Parse(data.growTime),

            bonus = float.Parse(data.bonus)
        };
    }

}

