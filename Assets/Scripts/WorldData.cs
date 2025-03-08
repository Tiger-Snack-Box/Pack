using UnityEngine;

[System.Serializable]
public class WorldData
{
    public string WorldName;
    public string PercentageComplete;
    public bool IsLocked;

    public WorldData(string WorldName, string PercentageComplete, bool IsLocked)
    {
        this.WorldName = WorldName;
        this.PercentageComplete = PercentageComplete;
        this.IsLocked = IsLocked;
    }
}
