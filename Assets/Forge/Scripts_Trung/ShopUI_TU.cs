//using UnityEngine;
//using System;     
//using System.Collections.Generic;
//using UnityEngine.UI;
//using TMPro;
///*
//*Script dùng cho cửa hàng trong game
//*
//*/
//public class ShopUI_TU : MonoBehaviour
//{
//    public enum Mode { Buy, Sell }

//    [Header("Data")]
//    public ShopCatalogSO catalog;
//    public PlayerWallet wallet;

//    [Header("UI - List hàng")]
//    public Transform shopListParent;        // ScrollView/Viewport/Content
//    public ShopItemRow_TU shopItemRowPrefab;     // Prefab 1 hàng
//    public TMP_Text titleText;             // “Cửa hàng”
//    public TMP_Text goldText; // 

//    [Header("UI - Popup")]
//    public ShopDetailPopUp_TU popup;

//    [Header("UI - Buttons dưới (chọn chế độ)")]
//    public Button btnBuyMode;
//    public Button btnSellMode;

//    [Header("Panels liên quan")]
//    [SerializeField] GameObject inventoryPanel;   // <— GÁN TRỰC TIẾP panel túi đồ!
//    public InventoryGrid inventoryGrid;           // grid trong inventoryPanel

//    // Callback cho Blacksmith khi shop đóng
//    [NonSerialized] public Action onShopClosed;

//    // Runtime
//    Mode mode = Mode.Buy;
//    readonly List<ShopItemRow_TU> rows = new();
//    ShopItemRow_TU currentRow;

//    float lastShopClickTime = 0f;
//    const float doubleClickThreshold = 0.30f;

//    InventorySlotCell currentInvSlot;
//    float lastInvClickTime = 0f;

//    void Awake()
//    {
//        if (btnBuyMode) btnBuyMode.onClick.AddListener(() => SetMode(Mode.Buy));
//        if (btnSellMode) btnSellMode.onClick.AddListener(() => SetMode(Mode.Sell));
//        if (popup) popup.Closed += OnPopupClosed;
//    }

//    void OnEnable()
//    {
//        if (!ValidateRefs()) return;

//        BuildRowsIfNeeded();

//        // Tránh listener bị cộng dồn khi bật/tắt panel nhiều lần:
//        inventoryGrid.onSlotClicked.RemoveAllListeners();

//        SetMode(Mode.Buy);
//        RefreshGold();
//    }

//    void OnDisable()
//    {
//        // Dọn listener khi tắt panel để không bị nhân đôi
//        if (inventoryGrid) inventoryGrid.onSlotClicked.RemoveAllListeners();
//    }

//    bool ValidateRefs()
//    {
//        if (!shopListParent) { Debug.LogError("ShopUI: shopListParent chưa gán."); return false; }
//        if (!shopItemRowPrefab) { Debug.LogError("ShopUI: shopItemRowPrefab chưa gán."); return false; }
//        if (!catalog) { Debug.LogError("ShopUI: catalog chưa gán."); return false; }
//        if (!wallet) { Debug.LogError("ShopUI: wallet chưa gán."); return false; }
//        if (!popup) { Debug.LogError("ShopUI: popup chưa gán."); return false; }
//        if (!inventoryGrid) { Debug.LogError("ShopUI: inventoryGrid chưa gán."); return false; }
//        if (!inventoryPanel) { Debug.LogError("ShopUI: inventoryPanel chưa gán."); return false; }
//        return true;
//    }

//    void BuildRowsIfNeeded()
//    {
//        if (rows.Count > 0) return;
//        if (catalog.items == null || catalog.items.Length == 0) return;

//        foreach (var it in catalog.items)
//        {
//            if (!it) continue;
//            var row = Instantiate(shopItemRowPrefab, shopListParent);
//            row.name = it.displayName;
//            row.Bind(it, it.buyPrice, OnClickBuyRow);
//            rows.Add(row);
//        }
//    }

//    public void SetMode(Mode m)
//    {
//        mode = m;

//        if (titleText) titleText.text = "Cửa hàng";

//        // reset chọn
//        if (currentRow) { currentRow.SetSelected(false); currentRow = null; }
//        if (currentInvSlot) { currentInvSlot.SetSelected(false); currentInvSlot = null; }

//        if (btnBuyMode) btnBuyMode.interactable = (mode != Mode.Buy);
//        if (btnSellMode) btnSellMode.interactable = (mode != Mode.Sell);

//        bool shopClickable = (mode == Mode.Buy);
//        foreach (var r in rows)
//        {
//            if (!r) continue;
//            r.SetInteractable(shopClickable);
//            if (!shopClickable) r.SetSelected(false);
//        }

//        // Reset listener grid theo mode (tránh cộng dồn)
//        inventoryGrid.onSlotClicked.RemoveAllListeners();

//        if (mode == Mode.Buy)
//        {
//            // Ở chế độ mua: inventory chỉ highlight — KHÔNG mở popup
//            inventoryGrid.onSlotClicked.AddListener(OnClickInventorySlotSelectOnly);
//        }
//        else
//        {
//            // Ở chế độ bán: đảm bảo panel inventory bật và gắn handler bán
//            if (inventoryPanel && !inventoryPanel.activeSelf) inventoryPanel.SetActive(true);
//            inventoryGrid.onSlotClicked.AddListener(OnClickInventorySlotToSell);
//        }

//        RefreshGold();
//    }

//    void RefreshGold()
//    {
//        if (goldText && wallet) goldText.text = $"Vàng: {wallet.Gold:0000}";
//    }

//    /* ======================= BUY ======================= */
//    void OnClickBuyRow(ShopItemRow_TU row)
//    {
//        float now = Time.time;

//        if (currentRow == row && (now - lastShopClickTime) < doubleClickThreshold)
//        {
//            OpenBuyPopup(row);
//        }
//        else
//        {
//            if (currentRow) currentRow.SetSelected(false);
//            currentRow = row;
//            currentRow.SetSelected(true);
//        }

//        lastShopClickTime = now;
//    }

//    public void BuySelectedOne()
//    {
//        if (mode != Mode.Buy || !currentRow) return;

//        var item = currentRow.Item;
//        int price = currentRow.Price;
//        int qty = 1;

//        int total = price * qty;
//        if (!wallet.TrySpend(total)) { Debug.Log("Không đủ vàng!"); return; }
//        if (!inventoryGrid.AddItem(item, qty)) { Debug.Log("Túi đầy!"); wallet.Add(total); return; }

//        RefreshGold();
//    }

//    void OpenBuyPopup(ShopItemRow_TU row)
//    {
//        int maxQty = row.Item.stackable ? 999 : 1;

//        popup.Open(row.Item, row.Price, "Mua", (item, qty) =>
//        {
//            int total = qty * row.Price;
//            if (!wallet.TrySpend(total)) { Debug.Log("Không đủ vàng!"); return; }
//            if (!inventoryGrid.AddItem(item, qty)) { Debug.Log("Túi đầy!"); wallet.Add(total); return; }
//            RefreshGold();
//        }, maxQty);
//    }

//    /* ======================= SELL ======================= */
//    void OnClickInventorySlotToSell(InventorySlotCell slot)
//    {
//        if (slot == null || slot.IsEmpty()) return;

//        float now = Time.time;

//        // Double-click cùng một ô trong ~0.3s => mở popup bán
//        if (currentInvSlot == slot && (now - lastInvClickTime) < doubleClickThreshold)
//        {
//            int unitSellPrice = Mathf.RoundToInt(slot.Item.buyPrice * catalog.sellRate);

//            popup.Open(slot.Item, unitSellPrice, "Bán", (it, qty) =>
//            {
//                if (!inventoryGrid.RemoveItem(it, qty)) { Debug.Log("Không đủ vật phẩm để bán"); return; }
//                wallet.Add(unitSellPrice * qty);
//                RefreshGold();
//            }, slot.Quantity);

//            // Cho phép người dùng chỉnh lại số lượng tối đa ngay khi mở
//            popup.SetMaxQty(slot.Quantity);
//        }
//        else
//        {
//            // Single click: chỉ highlight slot
//            SelectInventorySlot(slot);
//        }

//        lastInvClickTime = now;
//        Debug.Log($"[ShopUI] Sell click: {slot.Item?.displayName}, dt={Time.time - lastInvClickTime}");
//    }

//    void OnClickInventorySlotSelectOnly(InventorySlotCell slot)
//    {
//        if (slot == null) return;
//        SelectInventorySlot(slot);
//        Debug.Log($"[ShopUI] SelectOnly: {slot.Item?.displayName}");
//    }

//    void SelectInventorySlot(InventorySlotCell slot)
//    {
//        if (currentInvSlot) currentInvSlot.SetSelected(false);
//        currentInvSlot = slot;
//        currentInvSlot.SetSelected(true);
//    }

//    void OnPopupClosed() => RefreshGold();

//    /* ======================= CLOSE SHOP ======================= */
//    // GÁN hàm này vào OnClick của BtnClose (nút X) trên ShopPanel
//    public void CloseShop()
//    {
//        // Tắt InventoryPanel CHẮC CHẮN
//        if (inventoryPanel) inventoryPanel.SetActive(false);

//        // Tắt ShopPanel
//        gameObject.SetActive(false);

//        // Báo cho Blacksmith quay lại thoại
//        onShopClosed?.Invoke();
//    }


//}

