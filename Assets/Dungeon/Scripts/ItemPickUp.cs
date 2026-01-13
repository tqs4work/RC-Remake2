using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item itemData;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
    }
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Pickup();
    }

    private void Pickup()
    {
        ItemRuntime newItem = ItemRuntime.FromItem(itemData);        

        foreach (var invItem in PlayerRuntime.Instance.Player.Inventory)
        {
            if (invItem.itemID == newItem.itemID)
            {
                invItem.quantity += newItem.quantity;
                Debug.Log($"Picked up: {newItem.itemName} (Total: {invItem.quantity})");
                Destroy(gameObject);
                return;
            }
        }
        PlayerRuntime.Instance.Player.Inventory.Add(newItem);

        Debug.Log($"Picked up: {newItem.itemName}");

        Destroy(gameObject);
    }
}
