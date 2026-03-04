using UnityEngine;
using UnityEngine.UI;

public class StoneSlotCell : MonoBehaviour
{
    [Header("UI")]
    public Image icon;

    public ItemRuntime Item { get; private set; }

    public int StoneLevel
    {
        get
        {
            if (Item == null) return 0;
            return Item.upgradeLevel; // dùng upgradeLevel trong ItemRuntime
        }
    }

    void Awake()
    {
        SetEmpty();
    }

    public void SetEmpty()
    {
        Item = null;

        if (icon)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public void SetItem(ItemRuntime item)
    {
        Item = item;

        if (icon)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        var hover = GetComponent<ItemHoverHandler_TU>();
        if (hover == null)
            hover = gameObject.AddComponent<ItemHoverHandler_TU>();

        hover.Setup(item.itemData, item.upgradeLevel);
        hover.SetDurability(Mathf.RoundToInt(item.durability));
        hover.SetAnchor(GetComponent<RectTransform>());
    }

    public bool IsEmpty()
    {
        return Item == null;
    }
}