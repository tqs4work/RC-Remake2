using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Inventory/NewItem")]
public class Item : ScriptableObject
{
    [Header("Thông tin vật phẩm")]
    public string itemID;
    public string itemName;
    [TextArea]
    public string description;
    [Header("Hình ảnh")]
    public Sprite icon;
    [Header("Chỉ số vật phẩm")]
    public int quantity;
    public int price;
    public int level;
    [Header("Chỉ số chiến đấu")]
    public float atk;
    public float def;
    public float hp;
    public float mp;
    [Header("Chỉ số đặc biệt")]
    public float bonus;    

}
