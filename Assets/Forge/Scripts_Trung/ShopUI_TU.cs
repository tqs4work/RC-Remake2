using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI_TU : MonoBehaviour
{
    public enum Mode { Buy, Sell }

    [Header("Data")]
    public ShopCatalogSO_TU catalog;

    [Header("UI - List hàng")]
    public Transform shopListParent;
    public ShopItemRow_TU shopItemRowPrefab;
    public TMP_Text titleText;

    [Header("UI - Popup")]
    public ShopDetailPopUp_TU popup;

    [Header("Buttons")]
    public Button btnBuyMode;
    public Button btnSellMode;

    [NonSerialized] public Action onShopClosed;

    Mode mode = Mode.Buy;
    readonly List<ShopItemRow_TU> rows = new();
    ShopItemRow_TU currentRow;

    float lastClickTime = 0f;
    const float doubleClickThreshold = 0.3f;

    void Awake()
    {
        if (btnBuyMode) btnBuyMode.onClick.AddListener(() => SetMode(Mode.Buy));
        if (btnSellMode) btnSellMode.onClick.AddListener(() => SetMode(Mode.Sell));
    }

    void OnEnable()
    {
        BuildRowsIfNeeded();
        SetMode(Mode.Buy);
    }

    void BuildRowsIfNeeded()
    {
        if (rows.Count > 0) return;
        if (catalog.items == null) return;

        foreach (var it in catalog.items)
        {
            if (!it) continue;

            var row = Instantiate(shopItemRowPrefab, shopListParent);
            row.Bind(it, it.price, OnClickBuyRow);
            rows.Add(row);
        }
    }

    public void SetMode(Mode m)
    {
        mode = m;

        if (titleText)
            titleText.text = mode == Mode.Buy ? "Cửa hàng - Mua" : "Cửa hàng - Bán";

        if (btnBuyMode) btnBuyMode.interactable = (mode != Mode.Buy);
        if (btnSellMode) btnSellMode.interactable = (mode != Mode.Sell);
    }

    /* ======================= BUY ======================= */

    void OnClickBuyRow(ShopItemRow_TU row)
    {
        float now = Time.time;

        if (currentRow == row && (now - lastClickTime) < doubleClickThreshold)
        {
            OpenBuyPopup(row);
        }
        else
        {
            if (currentRow) currentRow.SetSelected(false);
            currentRow = row;
            currentRow.SetSelected(true);
        }

        lastClickTime = now;
    }

    void OpenBuyPopup(ShopItemRow_TU row)
    {
        int maxQty = row.Item.stackable ? 99 : 1;

        popup.Open(row.Item, row.Price, "Mua", (item, qty) =>
        {
            var player = PlayerRuntime.Instance.Player;

            int total = qty * row.Price;

            if (player.Gold < total)
            {
                Debug.Log("Không đủ vàng!");
                return;
            }

            // Trừ vàng
            player.Gold -= total;
            // Tạo runtime item từ ScriptableObject
            var runtimeItem = ItemRuntime.FromItem(item);

            // Set số lượng mua
            runtimeItem.quantity = qty;
            // lấy container type từ itemID
            var containerType = GetContainerType(item);
            player.Inventory[containerType].AddItem(runtimeItem);

        }, maxQty);
        popup.Close();
    }

    /* ======================= SELL ======================= */

    public void SellItem(ItemRuntime item, InventoryContainerType containerType, int quantity)
    {
        if (mode != Mode.Sell) return;

        var player = PlayerRuntime.Instance.Player;

        int unitSellPrice = Mathf.RoundToInt(item.price * catalog.sellRate);
        int total = unitSellPrice * quantity;

        if (!player.Inventory[containerType].items.Contains(item))
            return;

        item.quantity -= quantity;

        if (item.quantity <= 0)
            player.Inventory[containerType].items.Remove(item);

        player.Gold += total;
    }

    /* ======================= UTIL ======================= */

   InventoryContainerType GetContainerType(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Shovel:
            case ItemType.Axe:
            case ItemType.Pickaxe:
            case ItemType.WateringCan:
            case ItemType.Bow:
            case ItemType.Sword:
            case ItemType.Armor:
                return InventoryContainerType.Tool;

            case ItemType.Seed:
                return InventoryContainerType.Farm;

            case ItemType.Material:
            case ItemType.Stone:
            case ItemType.Arrow:
                return InventoryContainerType.Dungeon;

            case ItemType.Consumable:
                return InventoryContainerType.Hotbar;

            default:
                return InventoryContainerType.City;
        }
    }
    public void CloseShop()
    {
        gameObject.SetActive(false);

        if (currentRow)
            currentRow.SetSelected(false);

        currentRow = null;

        onShopClosed?.Invoke();
    }
}
