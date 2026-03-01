// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;

// public class RepairUI_TU : MonoBehaviour
// {
//    [Header("Root")]
//     public GameObject panelRoot;
//     public Button btnClose;

//     [Header("Confirm Dialog")]
//     public GameObject dialogConfirm;
//     public TMP_Text confirmText;
//     public Button btnConfirmOK, btnConfirmCancel;

//     [Header("Info Dialog")]
//     public GameObject dialogInfo;
//     public TMP_Text infoText;
//     public Button btnInfoOK;

//     [Header("Success Dialog")]
//     public GameObject dialogSuccess;
//     public TMP_Text successText;
//     public Button btnSuccessOK;

//     [Header("Loading")]
//     public GameObject loadingPanel;
//     public Slider loadingBar;
//     public TMP_Text loadingPercent;
//     public TMP_Text loadingTitle;   // Text "Đang sửa…"

//     [Header("Inventory")]
//     public InventoryUI inventoryUI;

//     ItemRuntime currentItem;              // item đang sửa (để hiển thị giá sửa trong tooltip)
//     Coroutine dotsCoroutine;               // giữ handle để dừng

//     public System.Action onClose;

//     void Awake()
//     {
//         HideAllDialogs();

//         if (btnClose)         btnClose.onClick.AddListener(Close);
//         if (btnConfirmOK)     btnConfirmOK.onClick.AddListener(DoStartRepair);
//         if (btnConfirmCancel) btnConfirmCancel.onClick.AddListener(() => dialogConfirm.SetActive(false));
//         if (btnInfoOK)        btnInfoOK.onClick.AddListener(() => dialogInfo.SetActive(false));
//         if (btnSuccessOK)     btnSuccessOK.onClick.AddListener(OnSuccessOK);

//         if (repairSlot)
//         {
//             repairSlot.SetEmpty();
//             var h = repairSlot.GetComponent<ItemHoverHandler>();
//             if (h) h.SetShowRepairPrice(true);
//         }
//     }

//     void HideAllDialogs()
//     {
//         // dừng dots nếu đang chạy
//         if (dotsCo != null) { StopCoroutine(dotsCo); dotsCo = null; }

//         if (dialogConfirm) dialogConfirm.SetActive(false);
//         if (dialogInfo)    dialogInfo.SetActive(false);
//         if (dialogSuccess) dialogSuccess.SetActive(false);
//         if (loadingPanel)  loadingPanel.SetActive(false);
//     }

//     public void Open()
//     {
//         var h = repairSlot.GetComponent<ItemHoverHandler>();
//         if (h) h.SetShowRepairPrice(true);

//         gameObject.SetActive(true);
//         if (panelRoot) panelRoot.SetActive(true);
//         HideAllDialogs();
//         ClearRepairSlot();
//     }

//     public void Close()
//     {
//         HideAllDialogs();
//         if (panelRoot) panelRoot.SetActive(false);
//         gameObject.SetActive(false);
//         onClose?.Invoke();
//     }

//     // ==== Nhận item từ Inventory (double-click/drag-drop) ====
//     public void TryPlaceFromInventory(InventorySlotCell invSlot)
//     {
//         if (invSlot == null || invSlot.IsEmpty())
//         {
//             ShowInfo("Không có vật phẩm để sửa.");
//             return;
//         }

//         var item = invSlot.Item;
//         if (item == null || !item.useDurability || item.type == ItemType.Arrow)
//         {
//             ShowInfo("Chỉ sửa được Kiếm, Giáp và Cung.");
//             return;
//         }

//         if (invSlot.DurabilityPercent >= 100)
//         {
//             ShowInfo("Độ bền vật phẩm đã đầy, không cần sửa.");
//             return;
//         }

//         _sourceSlot = invSlot;

//         repairSlot.SetItem(item, 1, invSlot.UpgradeLevel, invSlot.DurabilityPercent);
//         repairSlot.SetSelected(true);

//         _cachedPrice = item.GetRepairCost(invSlot.DurabilityPercent);
//         confirmText.text =
//             $"Sửa <b>{item.displayName}</b> từ <b>{invSlot.DurabilityPercent}%</b> → <b>100%</b>\n" +
//             $"Giá: <b>{_cachedPrice}</b> vàng.";
//         dialogConfirm.SetActive(true);
//     }

//     void ShowInfo(string msg)
//     {
//         infoText.text = msg;
//         dialogInfo.SetActive(true);
//     }

//     // ==== Thực hiện sửa ====
//     void DoStartRepair()
//     {
//         dialogConfirm.SetActive(false);

//         if (_sourceSlot == null || _sourceSlot.IsEmpty())
//         {
//             ShowInfo("Vật phẩm không còn trong túi đồ.");
//             return;
//         }

//         // Trừ vàng bằng PlayerWallet
//         if (wallet != null && !wallet.TrySpend(_cachedPrice))
//         {
//             ShowInfo("Bạn không đủ vàng để sửa.");
//             return;
//         }

//         StartCoroutine(CoLoadingRepair());
//     }

//     IEnumerator CoLoadingRepair()
//     {
//         // bật panel + reset thanh
//         loadingPanel.SetActive(true);
//         if (loadingBar)     loadingBar.value = 0;
//         if (loadingPercent) loadingPercent.text = "0%";
//         if (loadingTitle)   loadingTitle.text = "Đang sửa";

//         // chạy dấu chấm nhấp nháy
//         if (loadingTitle && dotsCo == null)
//             dotsCo = StartCoroutine(CoDots(loadingTitle));

//         // progress 0→100%
//         float t = 0f, dur = 1.1f;
//         while (t < dur)
//         {
//             t += Time.unscaledDeltaTime;
//             float p = Mathf.Clamp01(t / dur);
//             if (loadingBar)     loadingBar.value = p;
//             if (loadingPercent) loadingPercent.text = Mathf.RoundToInt(p * 100f) + "%";
//             yield return null;
//         }

//         // tắt dots + panel
//         if (dotsCo != null) { StopCoroutine(dotsCo); dotsCo = null; }
//         loadingPanel.SetActive(false);

//         // Cập nhật durability = 100 cho slot trong inventory + ô hiển thị
//         if (_sourceSlot != null) _sourceSlot.SetDurabilityPercent(100);
//         if (repairSlot   != null) repairSlot.SetDurabilityPercent(100);

//         successText.text = "Đã sửa thành công!";
//         dialogSuccess.SetActive(true);
//     }

//     // Bấm OK của "Sửa thành công!"
//     void OnSuccessOK()
//     {
//         dialogSuccess.SetActive(false);
//         ClearRepairSlot();   // làm ô sửa trống để nhận món khác
//     }

//     void ClearRepairSlot()
//     {
//         if (repairSlot != null)
//         {
//             repairSlot.SetEmpty();
//             var h = repairSlot.GetComponent<ItemHoverHandler>();
//             if (h) h.SetShowRepairPrice(true);
//         }
//         _sourceSlot = null;
//         _cachedPrice = 0;
//     }

//     // ====== Dots "Đang sửa..." ======
//     IEnumerator CoDots(TMP_Text t)
//     {
//         string baseStr = "Đang sửa";
//         while (loadingPanel && loadingPanel.activeSelf)
//         {
//             for (int i = 0; i < 4 && loadingPanel.activeSelf; i++)
//             {
//                 t.text = baseStr + new string('.', i);
//                 yield return new WaitForSecondsRealtime(0.25f);
//             }
//         }
//     }
// }
