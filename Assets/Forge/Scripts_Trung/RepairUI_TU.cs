using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RepairUI_TU : MonoBehaviour
{
    public static RepairUI_TU Instance;
    public InfoDialog_TU infoDialog;

    [Header("Root")]
    public GameObject panelRoot;
    public Button btnClose;
    [Header("Slot")]
    public RepairSlotCell_TU repairSlotCell;

    [Header("Confirm Dialog")]
    public GameObject dialogConfirm;
    public TMP_Text confirmText;
    public Button btnConfirmOK;
    public Button btnConfirmCancel;

    [Header("Info Dialog")]
    public GameObject dialogInfo;
    public TMP_Text infoText;
    public Button btnInfoOK;

    [Header("Success Dialog")]
    public GameObject dialogSuccess;
    public TMP_Text successText;
    public Button btnSuccessOK;

    [Header("Loading")]
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TMP_Text loadingPercent;
    public TMP_Text loadingTitle;
    RepairSlotCell_TU currentSlot;

    ItemRuntime currentItem;
    int repairPrice;
    Coroutine dotsCo;

    public System.Action onClose;

    void Awake()
    {
        Instance = this;
        HideAllDialogs();

        if (btnClose) btnClose.onClick.AddListener(Close);
        if (btnConfirmOK) btnConfirmOK.onClick.AddListener(StartRepair);
        if (btnConfirmCancel) btnConfirmCancel.onClick.AddListener(() => dialogConfirm.SetActive(false));
        if (btnInfoOK) btnInfoOK.onClick.AddListener(() => dialogInfo.SetActive(false));
        if (btnSuccessOK) btnSuccessOK.onClick.AddListener(OnSuccessOK);
    }

    void HideAllDialogs()
    {
        if (dotsCo != null)
        {
            StopCoroutine(dotsCo);
            dotsCo = null;
        }

        dialogConfirm?.SetActive(false);
        dialogInfo?.SetActive(false);
        dialogSuccess?.SetActive(false);
        loadingPanel?.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        panelRoot?.SetActive(true);
        HideAllDialogs();
    }

    public void Close()
    {
        HideAllDialogs();
        panelRoot?.SetActive(false);
        gameObject.SetActive(false);
        onClose?.Invoke();
    }

    // ===== Nhận item từ RepairSlot =====
    public void SetItem(ItemRuntime item, RepairSlotCell_TU slot)
    {
         if (item == null) return;

        currentItem = item;
        currentSlot = slot;

        repairPrice = CalculateRepairCost(item);

        confirmText.text =
            $"Sửa <b>{item.itemName}</b>\n" +
            $"Từ <b>{item.durability}%</b> → <b>100%</b>\n" +
            $"Giá: <b>{repairPrice}</b> vàng.";

        dialogConfirm.SetActive(true);
    }

    int CalculateRepairCost(ItemRuntime item)
    {
        return item.itemData.GetRepairCost(Mathf.RoundToInt(item.durability));
    }

    void ShowInfo(string msg)
    {
        infoText.text = msg;
        dialogInfo.SetActive(true);
    }

    void StartRepair()
    {
        dialogConfirm.SetActive(false);

        if (currentItem == null)
        {
          infoDialog?.Show("Không có vật phẩm.");
            return;
        }

        var player = PlayerRuntime.Instance.Player;

        if (player.Gold < repairPrice)
        {
           infoDialog?.Show("Bạn không đủ vàng.");
            return;
        }

        player.Gold -= repairPrice;
        btnConfirmOK.interactable = false;

        StartCoroutine(CoRepair());
    }

    IEnumerator CoRepair()
    {
        loadingPanel.SetActive(true);

        if (loadingBar) loadingBar.value = 0;
        if (loadingPercent) loadingPercent.text = "0%";
        if (loadingTitle) loadingTitle.text = "Đang sửa";

        if (dotsCo == null)
            dotsCo = StartCoroutine(CoDots());

        float t = 0f;
        float duration = 1.2f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);

            if (loadingBar) loadingBar.value = p;
            if (loadingPercent) loadingPercent.text = Mathf.RoundToInt(p * 100f) + "%";

            yield return null;
        }

        if (dotsCo != null)
        {
            StopCoroutine(dotsCo);
            dotsCo = null;
        }

        loadingPanel.SetActive(false);

        currentItem.durability = 100;

        successText.text = "Sửa thành công!";
        dialogSuccess.SetActive(true);

        btnConfirmOK.interactable = true;
    }

    void OnSuccessOK()
    {
        dialogSuccess.SetActive(false);

        if (currentSlot != null)
            currentSlot.SetEmpty();

        // 🔥 refresh inventory ngay
        var inv = FindObjectOfType<InventoryUI>();
        if (inv != null)
            inv.RefreshAll();

        currentItem = null;
        currentSlot = null;
    }

    IEnumerator CoDots()
    {
        string baseText = "Đang sửa";

        while (loadingPanel && loadingPanel.activeSelf)
        {
            for (int i = 0; i < 4 && loadingPanel.activeSelf; i++)
            {
                if (loadingTitle)
                    loadingTitle.text = baseText + new string('.', i);

                yield return new WaitForSecondsRealtime(0.25f);
            }
        }
    }
    public bool IsBlockingUI()
    {
        if (dialogSuccess != null && dialogSuccess.activeSelf)
            return true;

        if (dialogConfirm != null && dialogConfirm.activeSelf)
            return true;

        if (loadingPanel != null && loadingPanel.activeSelf)
            return true;

        return false;
    }
}