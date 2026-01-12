using UnityEngine;

public class InventoryItem_String_TA
{
    [Header("Thông tin vật phẩm")]
    public string idItem;
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

    public InventoryItem_String_TA(InventoryItem item)
    {
        idItem = item.idItem;
        itemName = item.itemName;
        description = item.description;
        icon = item.icon != null ? item.icon.name : "";
        quantity = item.quantity.ToString();
        price = item.price.ToString();
        level = item.level.ToString();
        atk = item.atk.ToString();
        def = item.def.ToString();
        hp = item.hp.ToString();
        mp = item.mp.ToString();
        bonus = item.bonus.ToString();
    }
}
