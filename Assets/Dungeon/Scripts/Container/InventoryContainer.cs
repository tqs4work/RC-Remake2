using System.Collections.Generic;

[System.Serializable]
public class InventoryContainer
{
    public InventoryContainerType containerType;
    public List<ItemRuntime> items = new();

    public void AddItem(ItemRuntime item)
    {
        items.Add(item);
    }
}