using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public GameObject highlight;

    private ItemRuntime currentItem;

    public void SetItem(ItemRuntime item)
    {
        currentItem = item;

        if (item == null)
        {
            icon.sprite = null;
            icon.enabled = false;
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = item.icon;

        amountText.text = item.isStackable && item.quantity > 1
            ? item.quantity.ToString()
            : "";
    }

    public ItemRuntime GetItem()
    {
        return currentItem;
    }

    public void SetHighlight(bool state)
    {
        highlight.SetActive(state);
    }
}