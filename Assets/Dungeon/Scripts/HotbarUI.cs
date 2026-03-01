using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public HotbarSlot[] slots;
    public int selectedIndex = 0;    
    
    HotbarData hotbarData;    

    void Start()
    {        
        hotbarData = PlayerRuntime.Instance.Player.Hotbar;        

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
        for (int i = 0; i < slots.Length; i++)
        {
            // ?? GÁN INDEX CHO SLOT
            slots[i].slotIndex = i;

            var item = hotbarData.slots[i];

            if (item != null)
                slots[i].SetItem(item);
            else
                slots[i].SetItem(null);
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
        return hotbarData.slots[selectedIndex];
    }
}