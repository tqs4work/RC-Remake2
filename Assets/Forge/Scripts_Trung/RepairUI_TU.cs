using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RepairUI_TU : MonoBehaviour
{
    public static RepairUI_TU Instance;
    [Header("Root")]
    public GameObject panelRoot;
    public Button btnClose;

    [Header("Item Display")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text durabilityText;

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

    // Runtime
    ItemRuntime currentItem;
    int repairPrice;
    Coroutine dotsCo;

    public System.Action onClose;

    void Awake()
    {
        HideAllDialogs();
        Instance = this;
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

        if (dialogConfirm) dialogConfirm.SetActive(false);
        if (dialogInfo) dialogInfo.SetActive(false);
        if (dialogSuccess) dialogSuccess.SetActive(false);
        if (loadingPanel) loadingPanel.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        if (panelRoot) panelRoot.SetActive(true);
        HideAllDialogs();
        ClearItem();
    }

    public void Close()
    {
        HideAllDialogs();
        if (panelRoot) panelRoot.SetActive(false);
        gameObject.SetActive(false);
        onClose?.Invoke();
    }

    // Nhận item
    public void SetItem(ItemRuntime item)
    {
        if (item == null)
        {
            ShowInfo("Không có vật phẩm.");
            return;
        }

        if (item.durability >= 100)
        {
            ShowInfo("Vật phẩm đã đầy độ bền.");
            return;
        }

        currentItem = item;

        if (itemIcon) itemIcon.sprite = item.icon;
        if (itemNameText) itemNameText.text = item.itemName;
        if (durabilityText) durabilityText.text = "Độ bền: " + item.durability + "%";

        repairPrice = CalculateRepairCost(item);

        confirmText.text =
            $"Sửa <b>{item.itemName}</b>\n" +
            $"Từ <b>{item.durability}%</b> → <b>100%</b>\n" +
            $"Giá: <b>{repairPrice}</b> vàng.";

        dialogConfirm.SetActive(true);
    }

    int CalculateRepairCost(ItemRuntime item)
    {
        float missing = 100f - item.durability;
        return Mathf.RoundToInt(item.price * (missing / 100f));
    }

    void ShowInfo(string msg)
    {
        infoText.text = msg;
        dialogInfo.SetActive(true);
    }

    // Thực hiện sửa
    void StartRepair()
    {
        dialogConfirm.SetActive(false);

        if (currentItem == null)
        {
            ShowInfo("Không có vật phẩm.");
            return;
        }

        var player = PlayerRuntime.Instance.Player;

        if (player.Gold < repairPrice)
        {
            ShowInfo("Bạn không đủ vàng.");
            return;
        }

        // Trừ vàng
        player.Gold -= repairPrice;

        // Chặn spam
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

        // FULL durability
        currentItem.durability = 100;
        InventoryUI.Instance?.RefreshAll();

        // Update lại text
        if (durabilityText)
            durabilityText.text = "Độ bền: 100%";

        successText.text = "Sửa thành công!";
        dialogSuccess.SetActive(true);

        btnConfirmOK.interactable = true;
    }

    void OnSuccessOK()
    {
        dialogSuccess.SetActive(false);
        ClearItem();
    }

    void ClearItem()
    {
        currentItem = null;

        if (itemIcon) itemIcon.sprite = null;
        if (itemNameText) itemNameText.text = "";
        if (durabilityText) durabilityText.text = "";
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
}