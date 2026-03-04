using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // ===== INVENTORY DICTIONARY =====
    public Dictionary<InventoryContainerType, InventoryContainer> Inventory = new();

    public List<ItemRuntime> Store = new();

    public List<ItemRuntime> Machine = new();

    public List<TileState> Wrapper = new();

    public List<ChickenData> ChickenWrapper = new();

    public Player()
    {
        Inventory[InventoryContainerType.Tool] =
            new InventoryContainer
            {
                containerType = InventoryContainerType.Tool,
                maxSize = 27        // ===== CHANGED =====
            };

        Inventory[InventoryContainerType.Farm] =
            new InventoryContainer
            {
                containerType = InventoryContainerType.Farm,
                maxSize = 27        // ===== CHANGED =====
            };

        Inventory[InventoryContainerType.City] =
            new InventoryContainer
            {
                containerType = InventoryContainerType.City,
                maxSize = 27        // ===== CHANGED =====
            };

        Inventory[InventoryContainerType.Dungeon] =
            new InventoryContainer
            {
                containerType = InventoryContainerType.Dungeon,
                maxSize = 27        // ===== CHANGED =====
            };

        Inventory[InventoryContainerType.Hotbar] =
            new InventoryContainer
            {
                containerType = InventoryContainerType.Hotbar,
                maxSize = 6
            };
    }

    // =====================================================
    // ===== NEW: STACK LOGIC (QUAN TRỌNG NHẤT) ============
    // =====================================================
    private bool TryStackOrAdd(InventoryContainer container, ItemRuntime item)
    {
        // ===== NEW: kiểm tra stack trước =====
        foreach (var invItem in container.items)
        {
            if (invItem.itemID == item.itemID && invItem.isStackable)
            {
                invItem.quantity += item.quantity;
                return true;
            }
        }

        // ===== NEW: kiểm tra full container =====
        if (container.maxSize > 0 &&
            container.items.Count >= container.maxSize)
            return false;

        container.items.Add(item);
        return true;
    }

    // =====================================================
    // ===== MOVE TO HOTBAR (STACK + SLOT FIXED) ==========
    // =====================================================
    public bool MoveToHotbar(
        InventoryContainerType fromContainer,
        ItemRuntime item,
        int hotbarIndex)
    {
        if (!Inventory.ContainsKey(fromContainer))
            return false;

        var source = Inventory[fromContainer];
        var hotbar = Inventory[InventoryContainerType.Hotbar];

        if (!source.items.Contains(item))
            return false;

        // ===== NEW: nếu hotbar slot đã có item =====
        if (hotbarIndex < hotbar.items.Count)
        {
            var existing = hotbar.items[hotbarIndex];

            // ===== NEW: nếu stack được thì cộng dồn =====
            if (existing != null &&
                existing.itemID == item.itemID &&
                existing.isStackable)
            {
                existing.quantity += item.quantity;
                source.items.Remove(item);
                return true;
            }

            // ===== NEW: nếu có item khác thì trả về source =====
            if (existing != null)
            {
                TryStackOrAdd(source, existing);
                hotbar.items[hotbarIndex] = null;
            }
        }

        // ===== NEW: đảm bảo list đủ size =====
        while (hotbar.items.Count <= hotbarIndex)
            hotbar.items.Add(null);

        hotbar.items[hotbarIndex] = item;
        source.items.Remove(item);

        return true;
    }

    // =====================================================
    // ===== MOVE FROM HOTBAR (STACK FIXED) ===============
    // =====================================================
    public bool MoveFromHotbar(int hotbarIndex, InventoryContainerType toContainer)
    {
        if (!Inventory.ContainsKey(toContainer))
            return false;

        var hotbar = Inventory[InventoryContainerType.Hotbar];
        var target = Inventory[toContainer];

        if (hotbarIndex >= hotbar.items.Count)
            return false;

        var item = hotbar.items[hotbarIndex];

        if (item == null)
            return false;

        // ===== CHANGED: dùng TryStackOrAdd thay vì AddItem =====
        bool success = TryStackOrAdd(target, item);

        if (!success)
            return false;

        hotbar.items[hotbarIndex] = null;

        return true;
    }

    // =====================================================
    // ===== MOVE GIỮA CONTAINER (STACK FIXED) ============
    // =====================================================
    public void MoveItem(
        InventoryContainerType from,
        InventoryContainerType to,
        ItemRuntime item)
    {
        if (!Inventory.ContainsKey(from)) return;
        if (!Inventory.ContainsKey(to)) return;

        var source = Inventory[from];
        var target = Inventory[to];

        if (!source.items.Contains(item)) return;

        // ===== CHANGED: dùng stack logic =====
        if (TryStackOrAdd(target, item))
        {
            source.items.Remove(item);
        }
    }

    // =====================================================
    // ===== LOAD DATA (HOTBAR FIXED) ======================
    // =====================================================
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
    { InventoryContainerType.Tool,
        new InventoryContainer
        {
            containerType = InventoryContainerType.Tool,
            maxSize = 27            // ===== CHANGED =====
        }
    },
    { InventoryContainerType.Farm,
        new InventoryContainer
        {
            containerType = InventoryContainerType.Farm,
            maxSize = 27            // ===== CHANGED =====
        }
    },
    { InventoryContainerType.City,
        new InventoryContainer
        {
            containerType = InventoryContainerType.City,
            maxSize = 27            // ===== CHANGED =====
        }
    },
    { InventoryContainerType.Dungeon,
        new InventoryContainer
        {
            containerType = InventoryContainerType.Dungeon,
            maxSize = 27            // ===== CHANGED =====
        }
    },
    { InventoryContainerType.Hotbar,
        new InventoryContainer
        {
            containerType = InventoryContainerType.Hotbar,
            maxSize = 6
        }
    }
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

        Store = data.Store;

        Machine = data.Machine;

        Wrapper = data.Wrapper;

        ChickenWrapper = data.ChickenWrapper;
    }    
}