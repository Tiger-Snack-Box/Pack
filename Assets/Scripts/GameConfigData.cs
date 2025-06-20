using System;
using System.Collections.Generic;

[Serializable]
public class GameConfigData
{
    public List<WorldConfigData> worlds;

    public GameConfigData(List<WorldConfigData> worlds)
    {
        this.worlds = worlds;
    }
}
