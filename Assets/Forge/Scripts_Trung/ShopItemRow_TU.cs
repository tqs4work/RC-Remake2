using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopItemRow_TU : MonoBehaviour
{
     [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public GameObject selectedBG;

    [SerializeField] Button button;

    Item item;
    int pricePerUnit;
    System.Action<ShopItemRow_TU> onClick;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (button) button.onClick.AddListener(() => onClick?.Invoke(this));
        SetSelected(false);
    }

    public void Bind(Item it, int price, System.Action<ShopItemRow_TU> clickCb)
    {
        item = it;
        pricePerUnit = price;
        onClick = clickCb;

        if (icon) icon.sprite = it ? it.icon : null;
        if (nameText) nameText.text = it ? it.itemName : "";
        if (priceText) priceText.text = price + "G";
        SetSelected(false);
        SetInteractable(true);
        var hover = GetComponent<ItemHoverHandler_TU>();
        if (hover == null) hover = gameObject.AddComponent<ItemHoverHandler_TU>();
        hover.Setup(it, 0);
        hover.SetDurability(100);
        hover.SetAnchor(GetComponent<RectTransform>());
    }

    public Item Item => item;
    public int Price => pricePerUnit;

    public void SetSelected(bool on)
    {
        if (selectedBG) selectedBG.SetActive(on);
    }

    public void SetInteractable(bool on)
    {
        if (!button) button = GetComponent<Button>();
        if (button) button.interactable = on;
    }
}
