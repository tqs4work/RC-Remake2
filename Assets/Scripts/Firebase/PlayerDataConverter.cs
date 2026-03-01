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
                       
            Inventory = new Dictionary<string, Dictionary<string, Item2String>>(),

            Wrapper = player.Wrapper != null ? new List<TileState>(player.Wrapper) : new List<TileState>(),
        };       
        
        foreach (var container in player.Inventory)
        {
            string containerName = container.Key.ToString();

            data.Inventory[containerName] =
                new Dictionary<string, Item2String>();

            foreach (var item in container.Value.items)
            {
                if (item == null || string.IsNullOrEmpty(item.itemID))
                    continue;

                data.Inventory[containerName][item.itemID] =
                    Item2String.FromRuntime(item);
            }
        }        

        return data;
    }
}
