using UnityEngine;
using System.Collections.Generic;
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<Item> allItems;

    void Awake()
    {
        Instance = this;
    }

    public Item GetItemByID(string id)
    {
        return allItems.Find(x => x.itemID == id);
    }
}
