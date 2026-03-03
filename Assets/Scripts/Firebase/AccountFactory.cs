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
            Mp = 100,
            Exp = 0,
            Lv = 1,
            Gold = 1000,
            IsOnline = false,
            LastLogin = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),

            // ? C?u trúc Inventory m?i

            Inventory = new Dictionary<string, Dictionary<string, Item2String>>(),

            Store = new List<ItemRuntime>(),

            Wrapper = new List<TileState>(),

            ChickenWrapper = new List<ChickenData>()
        };

        // T?o s?n 4 container r?ng
        player.Inventory["Tool"] = new Dictionary<string, Item2String>();
        player.Inventory["Farm"] = new Dictionary<string, Item2String>();
        player.Inventory["City"] = new Dictionary<string, Item2String>();
        player.Inventory["Dungeon"] = new Dictionary<string, Item2String>();

        return player;
    }
}