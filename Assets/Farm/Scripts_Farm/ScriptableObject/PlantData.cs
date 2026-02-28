using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[System.Serializable]
public class PlantStage
{
    public TileBase stageTile;
    [Tooltip("Thời gian để mọc xong stage này (giây)")]
    public float growthTimeInSeconds;
}
[CreateAssetMenu(fileName = "Seed_", menuName = "Farm/Seed Data")]
public class PlantData : ScriptableObject
{
    [Header("Seed Info")]
    public string seedName;
    public Sprite seedIcon;
    public int buyPrice;

    [Header("Growth")]
    public List<PlantStage> growthStages;

    [Header("Harvest")]
    [Tooltip("ID của VẬT PHẨM thu hoạch (phải khớp với ID trong InventoryItems)")]
    public string harvestItemID;
    public string harvestItemName;
    public string harvestItemDescription;
    public string harvestItemIconPath;
    public int harvestQuantity = 1;
    public int pricePerUnit;

    public TilemapState plantState;
    public bool IsHarvestable(TileBase tile)
    {
        if (growthStages == null || growthStages.Count == 0) return false;
        // Trả về true nếu 'tile' là tile cuối cùng trong danh sách
        return tile == growthStages[growthStages.Count - 1].stageTile;
    }
}
