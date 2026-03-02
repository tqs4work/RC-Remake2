using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item itemData;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Pickup();
    }

    public void Pickup()
    {
        ItemRuntime newItem = ItemRuntime.FromItem(itemData);
        var player = PlayerRuntime.Instance.Player;

        InventoryContainerType containerType =
            GetContainerType(newItem.itemID);

        var container = player.Inventory[containerType];

        // ======================================================
        // ===== 1?? KI?M TRA STACK TR??C =======================
        // ======================================================

        foreach (var invItem in container.items)
        {
            if (invItem.itemID == newItem.itemID &&
                invItem.isStackable)
            {
                invItem.quantity += newItem.quantity;

                Debug.Log($"Picked up: {newItem.itemName} (Stacked)");
                Destroy(gameObject);
                return;
            }
            else if (invItem.itemID == newItem.itemID &&
                     !invItem.isStackable)
            {
                newItem.itemID += " " + Random.Range(0f,100f).ToString();
                Destroy(gameObject);                
            }
        }

        // ======================================================
        // ===== 2?? KI?M TRA FULL TR??C KHI ADD ================
        // ======================================================

        if (container.maxSize > 0 &&
            container.items.Count >= container.maxSize)
        {
            Debug.Log("Inventory Full! Cannot pick up.");
            return; // ? KHÔNG Destroy ? v?n n?m trên ??t
        }

        // ======================================================
        // ===== 3?? ADD ITEM M?I ===============================
        // ======================================================

        container.items.Add(newItem);

        Debug.Log($"Picked up: {newItem.itemName}");
        Destroy(gameObject);
    }

    private InventoryContainerType GetContainerType(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return InventoryContainerType.Tool;

        char prefix = itemID[0];

        return prefix switch
        {
            'T' => InventoryContainerType.Tool,
            'F' => InventoryContainerType.Farm,
            'C' => InventoryContainerType.City,
            'D' => InventoryContainerType.Dungeon,
            _ => InventoryContainerType.Tool
        };
    }
}