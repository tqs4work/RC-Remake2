using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public HotbarSlot[] slots;
    public int selectedIndex = 0;

    List<ItemRuntime> inventory;

    void Start()
    {
        inventory = PlayerRuntime.Instance.Player.Inventory;
        Refresh();
        UpdateHighlight();
    }

    void Update()
    {
        HandleScroll();
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
        if (selectedIndex < inventory.Count)
            return inventory[selectedIndex];

        return null;
    }
}