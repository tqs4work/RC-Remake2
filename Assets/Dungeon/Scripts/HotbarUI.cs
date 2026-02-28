//using System.Collections.Generic;
//using UnityEngine;

//public class HotbarUI : MonoBehaviour
//{
//    public HotbarSlot[] slots;
//    public int selectedIndex = 0;

//    List<ItemRuntime> inventory;

//    void Start()
//    {
//        inventory = PlayerRuntime.Instance.Player.Inventory;
//        Refresh();
//        UpdateHighlight();
//    }

//    void Update()
//    {
//        HandleScroll();
//        Refresh();
//    }

//    void HandleScroll()
//    {
//        float scroll = Input.GetAxis("Mouse ScrollWheel");

//        if (scroll > 0)
//        {
//            selectedIndex++;
//            if (selectedIndex >= slots.Length)
//                selectedIndex = 0;

//            UpdateHighlight();
//        }
//        else if (scroll < 0)
//        {
//            selectedIndex--;
//            if (selectedIndex < 0)
//                selectedIndex = slots.Length - 1;

//            UpdateHighlight();
//        }
//    }

//    public void Refresh()
//    {
//        for (int i = 0; i < slots.Length; i++)
//        {
//            if (i < inventory.Count)
//                slots[i].SetItem(inventory[i]);
//            else
//                slots[i].SetItem(null);
//        }
//    }

//    void UpdateHighlight()
//    {
//        for (int i = 0; i < slots.Length; i++)
//        {
//            slots[i].SetHighlight(i == selectedIndex);
//        }
//    }

//    public ItemRuntime GetSelectedItem()
//    {
//        if (selectedIndex < inventory.Count)
//            return inventory[selectedIndex];

//        return null;
//    }
//}


using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public HotbarSlot[] slots;
    public int selectedIndex = 0;

    private List<ItemRuntime> inventory;

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

    public void Refresh()
    {
        var playerInventory = PlayerRuntime.Instance.Player.Inventory;

        // ?? Hotbar l?y t? Tool container
        if (!playerInventory.ContainsKey(InventoryContainerType.Tool))
            return;

        inventory = playerInventory[InventoryContainerType.Tool].items;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Count)
                slots[i].SetItem(inventory[i]);
            else
                slots[i].SetItem(null);
        }
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlight(i == selectedIndex);
        }
    }

    public ItemRuntime GetSelectedItem()
    {
        if (inventory == null)
            return null;

        if (selectedIndex < inventory.Count)
            return inventory[selectedIndex];

        return null;
    }
}