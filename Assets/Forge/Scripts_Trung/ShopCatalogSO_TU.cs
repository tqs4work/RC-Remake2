using UnityEngine;

[CreateAssetMenu(fileName = "ShopCatalogSO_TU", menuName = "Shop/Shop Catalog")]
public class ShopCatalogSO_TU : ScriptableObject
{
    public Item[] items;
    [Range(0f,1f)] public float sellRate = 0.5f; // bán lại 50% giá
}
