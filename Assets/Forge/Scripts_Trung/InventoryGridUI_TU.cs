using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryGridUI_TU : MonoBehaviour
{
     [Header("Setup")]
    public Transform gridParent;        // object có GridLayoutGroup
    public GameObject slotPrefab;       // prefab slot
    public int maxSlots = 25;

    private List<ItemRuntime> inventory;

    void OnEnable()
    {
        Render();
    }

    public void Render()
    {
        inventory = PlayerRuntime.Instance.Player.Inventory;

        // Xóa slot cũ
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }

        // Tạo slot mới
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);

            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            GameObject bg = slot.transform.Find("BG").gameObject;
            TMP_Text amount = slot.transform.Find("Amount").GetComponent<TMP_Text>();

            icon.sprite = null;
            icon.enabled = false;
            bg.SetActive(false);
            amount.text = "";

            if (i < inventory.Count)
            {
                var item = inventory[i];

                icon.sprite = item.icon;
                icon.enabled = true;

                if (item.isStackable && item.quantity > 1)
                {
                    amount.text = item.quantity.ToString();
                    bg.SetActive(true);
                }
            }
        }
    }
}
