// using UnityEngine;
// using System;     
// using System.Collections.Generic;
// using UnityEngine.UI;
// using TMPro;
// /*
// *Script dùng cho cửa hàng trong game
// *
// */
// public class ShopUI_TU : MonoBehaviour
// {  public enum Mode { Buy, Sell }

//     [Header("Data")]
//     public ShopCatalogSO_TU catalog;

//     [Header("UI - List hàng")]
//     public Transform shopListParent;
//     public ShopItemRow_TU shopItemRowPrefab;
//     public TMP_Text titleText;

//     [Header("UI - Popup")]
//     public ShopDetailPopUp_TU popup;

//     [Header("Buttons")]
//     public Button btnBuyMode;
//     public Button btnSellMode;

//     [Header("Inventory")]
//     public InventoryUI inventoryUI;

//     Mode mode = Mode.Buy;
//     readonly List<ShopItemRow_TU> rows = new();
//     ShopItemRow_TU currentRow;

//     void Awake()
//     {
//         if (btnBuyMode) btnBuyMode.onClick.AddListener(() => SetMode(Mode.Buy));
//         if (btnSellMode) btnSellMode.onClick.AddListener(() => SetMode(Mode.Sell));
//     }

//     void OnEnable()
//     {
//         BuildRowsIfNeeded();
//         SetMode(Mode.Buy);
//     }

//     void BuildRowsIfNeeded()
//     {
//         if (rows.Count > 0) return;
//         if (catalog.items == null || catalog.items.Length == 0) return;

//         foreach (var it in catalog.items)
//         {
//             if (!it) continue;

//             var row = Instantiate(shopItemRowPrefab, shopListParent);
//             row.name = it.itemName;
//             row.Bind(it, it.price, OnClickRow);
//             rows.Add(row);
//         }
//     }

//     public void SetMode(Mode m)
//     {
//         mode = m;

//         if (titleText)
//             titleText.text = mode == Mode.Buy ? "Cửa hàng - Mua" : "Cửa hàng - Bán";

//         if (btnBuyMode) btnBuyMode.interactable = (mode != Mode.Buy);
//         if (btnSellMode) btnSellMode.interactable = (mode != Mode.Sell);
//     }

//     void OnClickRow(ShopItemRow_TU row)
//     {
//         currentRow = row;

//         if (mode == Mode.Buy)
//             OpenBuyPopup(row);
//         else
//             Debug.Log("SELL mode chưa xử lý (InventoryUI chưa có RemoveItem)");
//     }

//     void OpenBuyPopup(ShopItemRow_TU row)
//     {
//         int maxQty = row.Item.stackable ? 99 : 1;

//         popup.Open(row.Item, row.Price, "Mua", (item, qty) =>
//         {
//             for (int i = 0; i < qty; i++)
//             {
//                 ItemRuntime runtimeItem = ItemRuntime.FromItem(item);
//                 PlayerRuntime.Instance.Player.Inventory.Add(runtimeItem);
//             }

//             Debug.Log("Đã thêm item vào inventory!");

//             // Refresh inventory UI
//             if (inventoryUI != null)
//                 inventoryUI.ShowToolPanel();
//         }, maxQty);
//     }
// }

