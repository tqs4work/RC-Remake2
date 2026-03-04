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
        Debug.Log($"Purchased {item.itemName} for {item.price} gold. Remaining gold: {currentMoney}");

        // Add item vào inventory
        //InventoryManager.Instance.AddItem(item);
    }
}