using UnityEngine;

[System.Serializable]
public class ItemRuntime
{
    public string itemID;
    public string itemName;
    public string description;
    public string icon;

    public int quantity;
    public int price;
    public int level;

    public float atk;
    public float def;
    public float hp;
    public float mp;
    public float bonus;

    public static ItemRuntime FromItem(Item item)
    {
        return new ItemRuntime
        {
            itemID = item.itemID,
            itemName = item.itemName,
            description = item.description,
            icon = item.icon != null ? item.icon.name : "",

            quantity = item.quantity,
            price = item.price,
            level = item.level,

            atk = item.atk,
            def = item.def,
            hp = item.hp,
            mp = item.mp,
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
            icon = data.icon,

            quantity = int.Parse(data.quantity),
            price = int.Parse(data.price),
            level = int.Parse(data.level),

            atk = float.Parse(data.atk),
            def = float.Parse(data.def),
            hp = float.Parse(data.hp),
            mp = float.Parse(data.mp),
            bonus = float.Parse(data.bonus)
        };
    }

}

