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

    public List<ItemRuntime> Inventory = new();    

    public void LoadFromData(PlayerData data)
    {
        ID = data.ID;
        Name = data.Name;
        Hp = data.Hp;
        Mp = data.Mp;
        Exp = data.Exp;
        Lv = data.Lv;
        Gold = data.Gold;        
        Inventory.Clear();
        if (data.Inventory == null) return;
        foreach (var item in data.Inventory)
            Inventory.Add(ItemRuntime.FromData(item.Value));
    }
}

