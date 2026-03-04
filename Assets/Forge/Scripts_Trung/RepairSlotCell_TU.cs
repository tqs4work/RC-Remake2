using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RepairSlotCell_TU : MonoBehaviour, IPointerClickHandler
{
    public Image icon;

    // 🔥 KÉO THẢ TRONG INSPECTOR
    public InfoDialog_TU infoDialog;

    ItemRuntime currentItem;

    public void SetItem(ItemRuntime item)
    {
        if (item == null) return;
        if (item.itemData == null) return;
        if (!item.itemData.useDurability) return;

        if (item.durability >= item.itemData.maxDurability)
        {
            infoDialog?.Show("Độ bền đã đầy, không cần sửa chữa.");
            return;
        }

        currentItem = item;
        icon.sprite = item.icon;
        icon.enabled = true;

        RepairUI_TU.Instance?.SetItem(item, this);
    }

    public void SetEmpty()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount >= 2)
            SetEmpty();
    }
}