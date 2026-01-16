using System;
using System.Collections.Generic;
using UnityEngine;

public static class AccountFactory
{
    public static AccountData Create(string username, string password)
    {
        return new AccountData
        {
            Username = username,
            Password = password,
            Timecreate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            Player = CreateNewPlayer()
        };
    }

    private static PlayerData CreateNewPlayer()
    {
        PlayerData player = new PlayerData
        {
            ID = Guid.NewGuid().ToString(),
            Name = "NewPlayer",
            Hp = 100,
            Mp = 50,
            Exp = 0,
            Lv = 1,
            Gold = 0,
            IsOnline = false,
            LastLogin = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            //Inventory = new List<Item2String>()
            Inventory = new Dictionary<string, Item2String>()
        };

        //Item item = ScriptableObject.CreateInstance<Item>();
        //StarterItemBuilder.Build(item);
        //ItemRuntime runtimeItem = ItemRuntime.FromItem(item);
        //Item2String itemData = Item2String.FromRuntime(runtimeItem);
        //// ? KEY = itemID
        //player.Inventory[item.itemID] = itemData;

        return player;
    }
}

