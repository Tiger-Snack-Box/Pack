using UnityEngine;

[System.Serializable]
public class WorldData
{
    public int WorldId;
    public string WorldName;
    public string PercentageComplete;
    public bool IsLocked;

    public WorldData(int worldId, string worldName, string percentageComplete, bool isLocked)
    {
        this.WorldId = worldId;
        this.WorldName = worldName;
        this.PercentageComplete = percentageComplete;
        this.IsLocked = isLocked;
    }
}
