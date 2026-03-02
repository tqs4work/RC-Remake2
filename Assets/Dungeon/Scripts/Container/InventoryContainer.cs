using System.Collections.Generic;

[System.Serializable]
public class InventoryContainer
{
    public InventoryContainerType containerType;


    //
    public int maxSize = -1;  // -1 = không gi?i h?n
    //


    public List<ItemRuntime> items = new();

    public void AddItem(ItemRuntime item)
    {
        //
        if (maxSize > 0 && items.Count >= maxSize)
        {            
            return;
        }
        //

        items.Add(item);
    }
}