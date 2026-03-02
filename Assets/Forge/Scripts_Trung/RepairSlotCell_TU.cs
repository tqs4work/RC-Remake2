using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RepairSlotCell_TU : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public Image icon;
    public TMP_Text durabilityText;

    ItemRuntime currentItem;

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void SetItem(ItemRuntime item)
    {
        if (item == null)
        {
            SetEmpty();
            return;
        }

        // Chỉ cho phép item có durability
        if (item.durability <= 0 && item.durability > 100)
        {
            Debug.Log("Item không hợp lệ.");
            return;
        }

        currentItem = item;

        if (icon)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }

        if (durabilityText)
            durabilityText.text = item.durability + "%";

        // Gửi sang RepairUI
        if (RepairUI_TU.Instance != null)
            RepairUI_TU.Instance.SetItem(item);
    }

    public void SetEmpty()
    {
        currentItem = null;

        if (icon)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (durabilityText)
            durabilityText.text = "";
    }

    // Double click để bỏ item khỏi slot
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount >= 2)
        {
            SetEmpty();
        }
    }
}
