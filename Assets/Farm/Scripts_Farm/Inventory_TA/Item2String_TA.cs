using UnityEngine;

public class Item2String
{
    [Header("Thông tin vật phẩm")]
    public ItemType itemType;
    public string itemID;
    public string itemName;

    [TextArea]
    public string description;

    [Header("Hình ảnh")]
    public string icon;

    [Header("Chỉ số vật phẩm")]
    public string isStackable;
    public string maxStack;
    public string price;
    public string level;

    [Header("Vật phẩm tiêu thụ")]
    public string quantity;
    public string hpAmount;
    public string mpAmount;

    [Header("Vật phẩm chiến đấu")]
    public string atk;
    public string def;
    public string durability;

    [Header("Hạt giống")]
    public string growTime;

    [Header("Chỉ số đặc biệt")]
    public string bonus;

    // ✅ BẮT BUỘC cho Firebase
    public Item2String() { }

    // ❌ KHÔNG dùng cho Firebase
   
    public static Item2String FromRuntime(ItemRuntime item)
    {
        return new Item2String
        {
            itemID = item.itemID,
            itemName = item.itemName,
            description = item.description,
            icon = item.icon.name,

            isStackable = item.isStackable.ToString(),
            maxStack = item.maxStack.ToString(),
            
            price = item.price.ToString(),
            level = item.level.ToString(),

            quantity = item.quantity.ToString(),
            hpAmount = item.hpAmount.ToString(),
            mpAmount = item.mpAmount.ToString(),

            atk = item.atk.ToString(),
            def = item.def.ToString(),
            durability = item.durability.ToString(),

            growTime = item.growTime.ToString(),

            bonus = item.bonus.ToString()
        };
    }

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
}
