using System;
using UnityEngine;


public enum TilemapState
{
    Ground,
    Grass,
    Forest,
    Wheat,
    Carrot,
    Potato,
    Beetroot,
    Radish,
    Cabbage,
    Kale,
    Parsnip,
    cauliflower,
    Pumpkin,
    Sunflower
}
public class TilemapDetail
{
    public int x { get; set; }
    public int y { get; set; }
    public TilemapState tilemapState { get; set; }
    public DateTime growTime { get; set; }
    public TilemapDetail()
    {

    }
    public TilemapDetail(int x, int y, TilemapState tilemapState, DateTime growTime)
    {
        this.x = x;
        this.y = y;
        this.tilemapState = tilemapState;
        this.growTime = growTime;
    }
}