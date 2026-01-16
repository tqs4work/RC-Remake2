using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Seed,
    Material
}
[CreateAssetMenu(fileName = "Item_", menuName = "Inventory/NewItem")]
public class Item : ScriptableObject
{
    [Header("Thông tin vật phẩm")]    
    public ItemType itemType;
    public string itemID;
    public string itemName;    
    
    [TextArea]
    public string description;

    [Header("Hình ảnh")]
    public Sprite icon;

    [Header("Chỉ số vật phẩm")]
    public bool isStackable;
    public int maxStack;
    public int price;
    public bool stackable = false;
    public int level;

    [Header("Vật phẩm tiêu thụ")]
    public int quantity;
    public int hpAmount;
    public int mpAmount;

    [Header("Vật phẩm chiến đấu")]
    public float atk;
    public float def;
    public float durability;

    [Header("Hạt giống")]
    public float growTime;    

    [Header("Chỉ số đặc biệt")]
    public float bonus;    

}
