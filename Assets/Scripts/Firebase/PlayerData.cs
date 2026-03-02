using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string ID;
    public string Name;
    public int Hp;
    public int Mp;
    public int Exp;
    public int Lv;
    public int Gold;
    public bool IsOnline;
    public string LastLogin;            

    //
    public Dictionary<string, Dictionary<string, Item2String>> Inventory;
    //    

    public List<TileState> Wrapper;
}
