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

    public void Pickup()
    {
        ItemRuntime newItem = ItemRuntime.FromItem(itemData);

        foreach (var invItem in PlayerRuntime.Instance.Player.Inventory)
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
                newItem.itemID = newItem.itemID + " " + Random.Range(0f, 1000f).ToString();
                Debug.Log($"Picked up: {newItem.itemName} (Total: {invItem.quantity})");
                PlayerRuntime.Instance.Player.Inventory.Add(newItem);
                Destroy(gameObject);
                return;
            }
        }
        PlayerRuntime.Instance.Player.Inventory.Add(newItem);

        Debug.Log($"Picked up: {newItem.itemName}");

        Destroy(gameObject);
    }
}
