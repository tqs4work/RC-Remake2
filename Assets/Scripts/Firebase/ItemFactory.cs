using UnityEngine;

public static class ItemFactory
{
    public static Item CreateFromData(Item2String data)
    {
        if (data == null)
        {
            Debug.LogError("Item2String data is null");
            return null;
        }

        Item item = ScriptableObject.CreateInstance<Item>();

        item.itemID = data.itemID;
        item.itemName = data.itemName;
        item.description = data.description;

        // ⚠ icon load theo cách của bạn (Resources / Addressables)
        item.icon = string.IsNullOrEmpty(data.icon)
            ? null
            : Resources.Load<Sprite>(data.icon);

        item.quantity = int.Parse(data.quantity);
        item.price = int.Parse(data.price);
        item.level = int.Parse(data.level);

        item.atk = int.Parse(data.atk);
        item.def = int.Parse(data.def);
        item.hp = int.Parse(data.hp);
        item.mp = int.Parse(data.mp);
        item.bonus = int.Parse(data.bonus);

        return item;
    }
}

