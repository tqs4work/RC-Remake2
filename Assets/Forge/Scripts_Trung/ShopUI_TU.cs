using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShopUI_TU : MonoBehaviour
{
    public enum Mode { Buy, Sell }

    [Header("Data")]
    public ShopCatalog_TU catalog;
    public PlayerWallet wallet;

    [Header("UI - Shop List")]
    public Transform shopListParent;
    public ShopItemRow_TU shopItemRowPrefab;
    public TMP_Text titleText;
    public TMP_Text goldText;

    [Header("UI - Popup")]
    public ShopDetailPopUp_TU popup;

    [Header("Mode Buttons")]
    public Button btnBuyMode;
    public Button btnSellMode;

    [Header("Inventory")]
    public GameObject inventoryPanel;
    public InventoryGridUI_TU inventoryUI;

    [NonSerialized] public Action onShopClosed;

    Mode mode = Mode.Buy;

    readonly List<ShopItemRow_TU> rows = new();
    ShopItemRow_TU currentRow;

    float lastClick;
    const float doubleClickTime = 0.3f;

    void Awake()
    {
        if (btnBuyMode) btnBuyMode.onClick.AddListener(() => SetMode(Mode.Buy));
        if (btnSellMode) btnSellMode.onClick.AddListener(() => SetMode(Mode.Sell));
    }

    void OnEnable()
    {
        BuildRowsIfNeeded();
        SetMode(Mode.Buy);
        RefreshGold();
    }

    /* ================= BUILD SHOP ================= */

    void BuildRowsIfNeeded()
    {
        if (rows.Count > 0) return;

        foreach (var it in catalog.items)
        {
            if (!it) continue;

            var row = Instantiate(shopItemRowPrefab, shopListParent);
            row.Bind(it, it.price, OnClickBuyRow);

            rows.Add(row);
        }
    }

    /* ================= MODE ================= */

    public void SetMode(Mode m)
    {
        mode = m;

        if (titleText) titleText.text = "Cửa hàng";

        if (btnBuyMode) btnBuyMode.interactable = (mode != Mode.Buy);
        if (btnSellMode) btnSellMode.interactable = (mode != Mode.Sell);

        bool shopClickable = mode == Mode.Buy;

        foreach (var r in rows)
        {
            if (!r) continue;
            r.SetInteractable(shopClickable);
            r.SetSelected(false);
        }

        if (inventoryPanel)
            inventoryPanel.SetActive(mode == Mode.Sell);

        RefreshGold();
    }

    void RefreshGold()
    {
        if (goldText && wallet)
            goldText.text = $"Vàng: {wallet.Gold:0000}";
    }

    /* ================= BUY ================= */

    void OnClickBuyRow(ShopItemRow_TU row)
    {
        float now = Time.time;

        if (currentRow == row && now - lastClick < doubleClickTime)
        {
            OpenBuyPopup(row);
        }
        else
        {
            if (currentRow) currentRow.SetSelected(false);
            currentRow = row;
            currentRow.SetSelected(true);
        }

        lastClick = now;
    }

    void OpenBuyPopup(ShopItemRow_TU row)
    {
        int maxQty = row.Item.isStackable ? 999 : 1;

        popup.Open(row.Item, row.Price, "Mua", (item, qty) =>
        {
            int total = qty * row.Price;

            if (!wallet.TrySpend(total))
            {
                Debug.Log("Không đủ vàng");
                return;
            }

            AddItemToInventory(item, qty);

            RefreshGold();

        }, maxQty);
    }

    void AddItemToInventory(Item item, int qty)
    {
        var inv = PlayerRuntime.Instance.Player.Inventory;

        if (item.isStackable)
        {
            foreach (var it in inv)
            {
                if (it.itemID == item.itemID)
                {
                    it.quantity += qty;
                    inventoryUI.Render();
                    return;
                }
            }
        }

        inv.Add(new ItemRuntime
        {
            itemID = item.itemID,
            icon = item.icon,
            quantity = qty,
            isStackable = item.isStackable
        });

        inventoryUI.Render();
    }

    /* ================= SELL ================= */

    public void SellItem(ItemRuntime item)
    {
        if (item == null) return;

        int price = Mathf.RoundToInt(item.price * catalog.sellRate);

        wallet.Add(price);

        PlayerRuntime.Instance.Player.Inventory.Remove(item);

        inventoryUI.Render();

        RefreshGold();
    }

    /* ================= CLOSE ================= */

    public void CloseShop()
    {
        if (inventoryPanel)
            inventoryPanel.SetActive(false);

        gameObject.SetActive(false);

        onShopClosed?.Invoke();
    }
}