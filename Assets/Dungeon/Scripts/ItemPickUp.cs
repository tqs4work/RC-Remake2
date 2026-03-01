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

        // Xác ??nh container theo itemID prefix
        InventoryContainerType containerType = GetContainerType(newItem.itemID);

        var container = player.Inventory[containerType];

        // Ki?m tra stack
        foreach (var invItem in container.items)
        {
            if (invItem.itemID == newItem.itemID && invItem.isStackable)
            {
                invItem.quantity += newItem.quantity;

                Debug.Log($"Picked up: {newItem.itemName} (Total: {invItem.quantity})");
                Destroy(gameObject);
                return;
            }

            if (invItem.itemID == newItem.itemID && !invItem.isStackable)
            {
                newItem.itemID = newItem.itemID + " " + Random.Range(0, 1000);

                container.items.Add(newItem);

                Debug.Log($"Picked up: {newItem.itemName}");
                Destroy(gameObject);
                return;
            }
        }

        // N?u ch?a t?n t?i
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
