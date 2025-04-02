using UnityEngine;

public class LevelData
{
    public int levelNumber { get; private set; }
    public int rating { get; set; }
    public bool isLocked { get; set; }

    public LevelData(int levelNumber, bool isLocked)
    {
        this.levelNumber = levelNumber;
        rating = 0;
        this.isLocked = isLocked;
    }

}
