using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Button slotPrefab;
    public int maxSlots = 25;
    public List<ItemRuntime> inventory;
    public GameObject inventoryPanel;

    private void Start()
    {
        RenderItems();
    }

    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            RenderItems();
        }            
    }

    void RenderItems()
    {
        inventory = PlayerRuntime.Instance.Player.Inventory;
        foreach (Transform child in slotPrefab.transform.parent)
        {
            if(child != slotPrefab.transform) Destroy(child.gameObject);
        }

        for(var i = 0; i < maxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, slotPrefab.transform.parent);
            var bgImg = slot.transform.Find("BG").GetComponent<Image>();
            var iconImg = slot.transform.Find("Icon").GetComponent<Image>();
            var amount = slot.transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            bgImg.gameObject.SetActive(false);
            iconImg.sprite = null;
            amount.text = string.Empty;

            slot.gameObject.SetActive(true);
                        
            Debug.Log($"Inventory Count: {inventory.Count}");
            if (i < inventory.Count)
            {
                var item = inventory[i];
                iconImg.sprite = item.icon;                
                amount.text = (item.quantity >= 1 && item.isStackable) ? item.quantity.ToString() : string.Empty;
                bgImg.gameObject.SetActive(item.quantity >= 1 && item.isStackable);
                iconImg.enabled = true;

            }



        }
    }
}
