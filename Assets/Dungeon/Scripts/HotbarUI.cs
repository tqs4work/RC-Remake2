using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public HotbarSlot[] slots;
    public int selectedIndex = 0;    

    void Start()
    {        
        Refresh();
        UpdateHighlight();
    }

    void Update()
    {
        HandleScroll();
        Refresh();
    }

    void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0)
        {
            selectedIndex++;
            if (selectedIndex >= slots.Length)
                selectedIndex = 0;

            UpdateHighlight();
        }
        else if (scroll < 0)
        {
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = slots.Length - 1;

            UpdateHighlight();
        }
    }    

    //
    public void Refresh()
    {
        // ===== CHANGED: L?y container tr?c ti?p m?i l?n =====
        var hotbarContainer =
            PlayerRuntime.Instance.Player
            .Inventory[InventoryContainerType.Hotbar];

        for (int i = 0; i < slots.Length; i++)
        {
            // ?? GÁN INDEX CHO SLOT
            slots[i].slotIndex = i;

            ItemRuntime item = null;

            if (i < hotbarContainer.items.Count)
                item = hotbarContainer.items[i];

            slots[i].SetItem(item);
        }
    }
    //

    void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlight(i == selectedIndex);
        }
    }    
    
    public ItemRuntime GetSelectedItem()
    {
        var hotbarContainer =
            PlayerRuntime.Instance.Player
            .Inventory[InventoryContainerType.Hotbar];

        // ===== CHANGED: l?y t? container =====
        if (selectedIndex < hotbarContainer.items.Count)
            return hotbarContainer.items[selectedIndex];

        return null;
    }
}