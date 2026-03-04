using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PharmacyUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button bigMPButton;
    [SerializeField] private Button bigHPButton;

    [Header("Items (ScriptableObject)")]
    [SerializeField] private Item bigMPItem;
    [SerializeField] private Item bigHPItem;

    //Money
    private int currentMoney;

    private void Start()
    {
        currentMoney = PlayerRuntime.Instance.Player.Gold;
    }

    public void BuyItem(Item item)
    {
        MoneyCheck(item);
    }

    public void MoneyCheck(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Item is null!");
            return;
        }

        // Thử trừ tiền
        bool success = CurrencyMana.Instance.SpendCoin(item.price);
        Debug.Log($"Attempting to purchase {item.itemName} for {item.price} gold. Success: {success}");

        if (!success)
        {
            Debug.Log("Not enough gold!");
            return;
        }

        // Nếu trừ tiền thành công
        currentMoney = PlayerRuntime.Instance.Player.Gold;
        AddItem(item);
        Debug.Log($"Purchased {item.itemName} for {item.price} gold. Remaining gold: {currentMoney}");

        // Add item vào inventory
        //InventoryManager.Instance.AddItem(item);
    }

    public void AddItem (Item item)
    {
        ItemRuntime newItem = ItemRuntime.FromItem(item);
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
                if(invItem.quantity + newItem.quantity > invItem.maxStack)
                {
                    int spaceLeft = invItem.maxStack - invItem.quantity;
                    invItem.quantity += spaceLeft;
                    newItem.quantity -= spaceLeft;
                    newItem.itemID += " " + Random.Range(0f, 100f).ToString();
                    Debug.Log($"Picked up: {newItem.itemName} (Partially Stacked)");
                }
                else
                {
                    invItem.quantity += newItem.quantity;                    
                    Debug.Log($"Picked up: {newItem.itemName} (Stacked)");
                    //Destroy(gameObject);
                    return;
                }                
                
            }
            else if (invItem.itemID == newItem.itemID &&
                     !invItem.isStackable)
            {
                newItem.itemID += " " + Random.Range(0f,100f).ToString();
                //Destroy(gameObject);                
            }
        }

        // ======================================================
        // ===== 2?? KI?M TRA FULL TR??C KHI ADD ================
        // ======================================================

        if (container.maxSize > 0 &&
            container.items.Count >= container.maxSize)
        {
            Debug.Log("Inventory Full! Cannot pick up.");
            return; // ? KH�NG Destroy ? v?n n?m tr�n ??t
        }

        // ======================================================
        // ===== 3?? ADD ITEM M?I ===============================
        // ======================================================

        container.items.Add(newItem);

        Debug.Log($"Picked up: {newItem.itemName}");
        //Destroy(gameObject);
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