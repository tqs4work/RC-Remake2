using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponUpgradeSlotCell_TU : MonoBehaviour, IPointerClickHandler
{
    public enum SlotType
    {
        Weapon,   // Slot A
        Stone,    // Slot B
        Result    // Slot C
    }

    public SlotType slotType;

    public Image icon;
    public TMP_Text amountText;

    public ItemRuntime currentItem;
    public int upgradeLevel;

    WeaponUpgradeUI_TU upgradeUI;

    void Awake()
    {
        upgradeUI = GetComponentInParent<WeaponUpgradeUI_TU>();
        Clear();
    }

    public void SetItem(ItemRuntime item, int level = 0)
    {
        currentItem = item;
        upgradeLevel = level;

        icon.sprite = item.icon;
        icon.enabled = true;

        if (item.isStackable && item.quantity > 1)
        {
            amountText.text = item.quantity.ToString();
        }
        else
        {
            amountText.text = "";
        }
    }

    public void Clear()
    {
        currentItem = null;
        upgradeLevel = 0;
        icon.sprite = null;
        icon.enabled = false;
        amountText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotType == SlotType.Result)
            return;

        if (currentItem == null)
            return;

        upgradeUI.OnSlotClicked(this);
    }
}