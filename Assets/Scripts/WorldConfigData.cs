using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldConfigData
{
    public int worldNumber;
    public string worldName;
    public int coins;
    public List<LevelConfigData> levels;

    public WorldConfigData(int worldNumber, string worldName, int coins, List<LevelConfigData> levels)
    {
        this.worldNumber = worldNumber;
        this.worldName = worldName;
        this.coins = coins;
        this.levels = levels;
    }
}
