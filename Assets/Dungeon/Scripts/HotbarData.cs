using System;
using UnityEngine;

[Serializable]
public class HotbarData
{
    public ItemRuntime[] slots;

    public HotbarData(int size)
    {
        slots = new ItemRuntime[size];
    }

    public void SetItem(int index, ItemRuntime item)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index] = item;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index] = null;
    }

    public int GetFirstEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                return i;
        }
        return -1;
    }

    public void RemoveReference(ItemRuntime item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == item)
                slots[i] = null;
        }
    }
}