using UnityEngine;

[CreateAssetMenu(fileName = "ShopCatalog", menuName = "RPG/ShopCatalog")]
public class ShopCatalog_TU : ScriptableObject
{
    public Item[] items;
    [Range(0f,1f)] public float sellRate = 0.5f; // bán lại 50% giá
}
