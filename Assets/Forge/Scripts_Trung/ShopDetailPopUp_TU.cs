using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class ShopDetailPopUp_TU : MonoBehaviour
{
        [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text pricePerUnitText;
    public TMP_Text totalText;

    public Button btnMinus;
    public Button btnPlus;
    public TMP_InputField qtyInput;

    public Button btnConfirm;
    public Button btnClose;

    // runtime
    Item item;
    int unitPrice;
    int qty = 1;
    int maxQty = 99;
    Action<Item, int> onConfirm;

    public event Action Closed;  // ✅ Cho ShopUI biết khi popup đóng
    public bool IsOpen => gameObject.activeSelf;

    void Awake()
    {
        if (btnMinus) btnMinus.onClick.AddListener(() => SetQty(qty - 1));
        if (btnPlus)  btnPlus.onClick.AddListener(() => SetQty(qty + 1));

        if (qtyInput)
        {
            qtyInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            qtyInput.onEndEdit.AddListener(OnEditQty);
        }

        if (btnClose)   btnClose.onClick.AddListener(Close);
        if (btnConfirm) btnConfirm.onClick.AddListener(ConfirmAndClose);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Mở popup chi tiết mua/bán
    /// </summary>
    public void Open(Item it, int pricePerUnit, string confirmLabel,
                     Action<Item, int> onConfirmCb, int maxQtyClamp = 99)
    {
        item = it;
        unitPrice = Mathf.Max(0, pricePerUnit);
        onConfirm = onConfirmCb;
        maxQty = Mathf.Max(1, maxQtyClamp);

        if (icon) icon.sprite = it ? it.icon : null;
        if (nameText) nameText.text = it ? it.itemName : "";
        if (pricePerUnitText) pricePerUnitText.text = $"{unitPrice}G / món";
        if (btnConfirm)
        {
            var text = btnConfirm.GetComponentInChildren<TMP_Text>();
            if (text) text.text = confirmLabel;
        }

        SetQty(1); // mặc định
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Đặt lại số lượng tối đa khi mở popup từ Inventory (khi bán)
    /// </summary>
    public void SetMaxQty(int newMax)
    {
        maxQty = Mathf.Max(1, newMax);
        SetQty(Mathf.Min(qty, maxQty));
    }

    void OnEditQty(string s)
    {
        if (!int.TryParse(s, out var v)) v = qty;
        SetQty(v);
    }

    void SetQty(int v)
    {
        qty = Mathf.Clamp(v, 1, maxQty);

        if (qtyInput && qtyInput.text != qty.ToString())
            qtyInput.text = qty.ToString();

        if (totalText)
            totalText.text = $"Tổng: {qty * unitPrice}G";

        UpdateButtonsState();
    }

    void UpdateButtonsState()
    {
        if (btnMinus) btnMinus.interactable = (qty > 1);
        if (btnPlus)  btnPlus.interactable  = (qty < maxQty);
    }

    void ConfirmAndClose()
    {
        onConfirm?.Invoke(item, qty);
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        onConfirm = null;
        item = null;
        Closed?.Invoke();  // ✅ báo cho ShopUI biết popup đã đóng
    }
}
