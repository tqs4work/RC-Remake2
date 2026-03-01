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

    public List<TileState> Wrapper = new();


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

        Hotbar = new HotbarData(6); // 6 slot
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

    public HotbarData Hotbar;

    public bool MoveToHotbar(InventoryContainerType fromContainer, ItemRuntime item, int hotbarIndex)
    {
        if (!Inventory.ContainsKey(fromContainer))
            return false;

        if (!Inventory[fromContainer].items.Contains(item))
            return false;

        // N?u hotbar slot ?ã có item ? tr? v? container c?
        if (Hotbar.slots[hotbarIndex] != null)
        {
            Inventory[fromContainer].AddItem(Hotbar.slots[hotbarIndex]);
        }

        // Remove kh?i container
        Inventory[fromContainer].items.Remove(item);

        // Gán vào hotbar
        Hotbar.slots[hotbarIndex] = item;

        return true;
    }

    public bool MoveFromHotbar(int hotbarIndex, InventoryContainerType toContainer)
    {
        if (!Inventory.ContainsKey(toContainer))
            return false;

        var item = Hotbar.slots[hotbarIndex];
        if (item == null)
            return false;

        Inventory[toContainer].AddItem(item);

        Hotbar.slots[hotbarIndex] = null;

        return true;
    }

    public void MoveItem(
    InventoryContainerType from,
    InventoryContainerType to,
    ItemRuntime item)
    {
        if (!Inventory.ContainsKey(from)) return;
        if (!Inventory.ContainsKey(to)) return;

        if (Inventory[from].items.Contains(item))
        {
            Inventory[from].items.Remove(item);
            Inventory[to].items.Add(item);
        }
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
    }
}

