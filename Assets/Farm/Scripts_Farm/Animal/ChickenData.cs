using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChickenData
{
    public float posX, posY, posZ;
    public ChicManager.ChicState state;
    public float currentTimer;
    public bool isHungryStatus;
    public int feedCount;
    public int eggsProduced;
}
[System.Serializable]
public class ChickenListWrapper
{
    public List<ChickenData> chickens = new List<ChickenData>();
}
