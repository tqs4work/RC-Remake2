using System.Collections.Generic;
using UnityEngine;

public static class PlayerDataConverter
{
    public static PlayerData ToData(Player player)
    {
        PlayerData data = new PlayerData
        {
            ID = player.ID,
            Name = player.Name,
            Hp = player.Hp,
            Mp = player.Mp,
            Exp = player.Exp,
            Lv = player.Lv,
            Gold = player.Gold,            
            Inventory = new Dictionary<string, Item2String>()
        };
       

        foreach (var item in player.Inventory)
        {
            if (item == null || string.IsNullOrEmpty(item.itemID))
                continue;

            // key = itemID
            data.Inventory[item.itemID] = Item2String.FromRuntime(item);
        }

        return data;
    }
}
