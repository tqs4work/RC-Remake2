using UnityEngine;

public static class StarterItemBuilder
{
    public static void Build(Item item)
    {
        item.itemID = "item001";
        item.itemName = "Starter Sword";
        item.description = "A basic sword for new adventurers.";
        item.quantity = 1;
        item.price = 100;
        item.level = 1;
        item.atk = 10;
    }
}
