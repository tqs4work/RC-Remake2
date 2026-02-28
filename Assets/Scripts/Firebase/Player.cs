using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string ID;
    public string Name;
    public int Hp;
    public int Mp;
    public int Exp;
    public int Lv;
    public int Gold;

    //public List<ItemRuntime> Inventory = new();    

    //
    public Dictionary<InventoryContainerType, InventoryContainer> Inventory
    = new();


    public Player()
    {
        Inventory[InventoryContainerType.Tool] =
            new InventoryContainer { containerType = InventoryContainerType.Tool };

        Inventory[InventoryContainerType.Farm] =
            new InventoryContainer { containerType = InventoryContainerType.Farm };

        Inventory[InventoryContainerType.City] =
            new InventoryContainer { containerType = InventoryContainerType.City };

        Inventory[InventoryContainerType.Dungeon] =
            new InventoryContainer { containerType = InventoryContainerType.Dungeon };
    }


    private InventoryContainerType GetContainerType(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return InventoryContainerType.Tool;

        char prefix = itemID[0];

        return prefix switch
        {
            'T' => InventoryContainerType.Tool,
            'F' => InventoryContainerType.Farm,
            'C' => InventoryContainerType.City,
            'D' => InventoryContainerType.Dungeon,
            _ => InventoryContainerType.Tool
        };
    }

    public void AddItem(ItemRuntime item)
    {
        var containerType = GetContainerType(item.itemID);
        Inventory[containerType].AddItem(item);
    }

    public void InitializeInventory()
    {
        Inventory = new Dictionary<InventoryContainerType, InventoryContainer>
    {
        { InventoryContainerType.Tool, new InventoryContainer() },
        { InventoryContainerType.Farm, new InventoryContainer() },
        { InventoryContainerType.City, new InventoryContainer() },
        { InventoryContainerType.Dungeon, new InventoryContainer() }
    };
    }

    //

    public void LoadFromData(PlayerData data)
    {
        ID = data.ID;
        Name = data.Name;
        Hp = data.Hp;
        Mp = data.Mp;
        Exp = data.Exp;
        Lv = data.Lv;
        Gold = data.Gold;


        //Inventory.Clear();
        //if (data.Inventory == null) return;
        //foreach (var item in data.Inventory)
        //    Inventory.Add(ItemRuntime.FromData(item.Value));


        //
        Inventory = new Dictionary<InventoryContainerType, InventoryContainer>
        {
            { InventoryContainerType.Tool, new InventoryContainer { containerType = InventoryContainerType.Tool } },
            { InventoryContainerType.Farm, new InventoryContainer { containerType = InventoryContainerType.Farm } },
            { InventoryContainerType.City, new InventoryContainer { containerType = InventoryContainerType.City } },
            { InventoryContainerType.Dungeon, new InventoryContainer { containerType = InventoryContainerType.Dungeon } }
        };

        if (data.Inventory == null)
            return;

        foreach (var containerPair in data.Inventory)
        {
            InventoryContainerType type =
                Enum.Parse<InventoryContainerType>(containerPair.Key);

            if (!Inventory.ContainsKey(type))
                continue;

            foreach (var itemPair in containerPair.Value)
            {
                Inventory[type].items.Add(
                    ItemRuntime.FromData(itemPair.Value)
                );
            }
        }
        //
    }
}

