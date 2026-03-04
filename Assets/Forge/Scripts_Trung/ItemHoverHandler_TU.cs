using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHoverHandler_TU : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item item;
    [Range(0,5)] public int upgradeLevel = 0;
    [SerializeField] RectTransform anchor;
    public System.Action onForwardClick;

    [Range(0,100)] public int durabilityPercent = 100;
    public bool showRepairPrice = false;
    public void SetShowRepairPrice(bool on) => showRepairPrice = on;

    RectTransform selfRT;
    void Awake() => selfRT = transform as RectTransform;

    public void Setup(Item it, int level)
    {
        item = it; upgradeLevel = Mathf.Clamp(level, 0, 5);
    }
    public void SetDurability(int p) => durabilityPercent = Mathf.Clamp(p, 0, 100);
    public void SetAnchor(RectTransform target) => anchor = target;

    int GetCurrentDurability()
    {
        return durabilityPercent;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!item) return;

        // 🔥 Nếu đang có popup sửa thành công → không cho hover
        if (RepairUI_TU.Instance != null &&
            RepairUI_TU.Instance.IsBlockingUI())
            return;
        if (StoneUpgradeUI_TU.Instance != null &&
        StoneUpgradeUI_TU.Instance.IsBlockingUI())
        return;
        var tt = ItemTooltipUI_TU.Ensure(); // <-- đảm bảo có instance kể cả khi object bị tắt
        if (tt == null) return;

        var a = anchor ? anchor : selfRT;
        int dur = GetCurrentDurability();
        tt.Show(item, upgradeLevel, a, dur, showRepairPrice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI_TU.Ensure()?.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemTooltipUI_TU.Ensure()?.Hide();
        onForwardClick?.Invoke();
    }
}