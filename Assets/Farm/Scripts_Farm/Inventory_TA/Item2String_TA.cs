using UnityEngine;

public class Item2String
{
    [Header("Thông tin vật phẩm")]
    public string itemID;
    public string itemName;
    [TextArea]
    public string description;
    [Header("Hình ảnh")]
    public string icon;
    [Header("Chỉ số vật phẩm")]
    public string quantity;
    public string price;
    public string level;
    [Header("Chỉ số chiến đấu")]
    public string atk;
    public string def;
    public string hp;
    public string mp;
    [Header("Chỉ số đặc biệt")]
    public string bonus;    

    // ✅ BẮT BUỘC cho Firebase
    public Item2String() { }

    // ❌ KHÔNG dùng cho Firebase

    //public static Item2String FromItem(Item item)
    //{
    //    if (item == null)
    //    {
    //        Debug.LogError("Item is null when converting to Item2String");
    //        return null;
    //    }

    //    return new Item2String
    //    {
    //        itemID = item.itemID,
    //        itemName = item.itemName,
    //        description = item.description,
    //        icon = item.icon != null ? item.icon.name : "",
    //        quantity = item.quantity.ToString(),
    //        price = item.price.ToString(),
    //        level = item.level.ToString(),
    //        atk = item.atk.ToString(),
    //        def = item.def.ToString(),
    //        hp = item.hp.ToString(),
    //        mp = item.mp.ToString(),
    //        bonus = item.bonus.ToString()
    //    };
    //}

    public static Item2String FromRuntime(ItemRuntime item)
    {
        return new Item2String
        {
            itemID = item.itemID,
            itemName = item.itemName,
            description = item.description,
            icon = item.icon,

            quantity = item.quantity.ToString(),
            price = item.price.ToString(),
            level = item.level.ToString(),

            atk = item.atk.ToString(),
            def = item.def.ToString(),
            hp = item.hp.ToString(),
            mp = item.mp.ToString(),
            bonus = item.bonus.ToString()
        };
    }
}
