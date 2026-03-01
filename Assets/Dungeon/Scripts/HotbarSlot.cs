using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HotbarSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public GameObject highlight;

    private ItemRuntime currentItem;

    public int slotIndex;


    bool isOpenInventory;
    void Update()
    {
        isOpenInventory = GameObject.Find("Canvas").transform.Find("InventoryUI").GetComponent<InventoryUI>().isPanelOpen;
    }
    public void SetItem(ItemRuntime item)
    {
        currentItem = item;

        if (item == null)
        {
            icon.sprite = null;
            icon.enabled = false;
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = item.icon;

        amountText.text = item.isStackable && item.quantity >= 1
            ? item.quantity.ToString()
            : "";
    }

    public ItemRuntime GetItem()
    {
        return currentItem;
    }

    public void SetHighlight(bool state)
    {
        highlight.SetActive(state);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        if (eventData.clickCount == 2 && isOpenInventory)
        {
            var player = PlayerRuntime.Instance.Player;

            string itemID = currentItem.itemID;            

            // ===== CHANGED: L?y container Hotbar =====
            var hotbarContainer =
                player.Inventory[InventoryContainerType.Hotbar];

            // ===== CHANGED: Ki?m tra index h?p l? =====
            if (slotIndex < hotbarContainer.items.Count)
            {
                var item = hotbarContainer.items[slotIndex];

                // ===== CHANGED: Move t? Hotbar container v? container g?c =====
                player.MoveItem(
                    InventoryContainerType.Hotbar,
                    GetContainerType(itemID),
                    item
                );
            }
            //

            FindFirstObjectByType<HotbarUI>()?.Refresh();
            FindFirstObjectByType<InventoryUI>()?.RefreshAll();
        }
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
