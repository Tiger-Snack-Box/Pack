using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelConfigData
{
    public int levelNumber;
    public float snackQueueDuration;
    public int possibleSnackCount;
    public float shiftDuration;
    public int possiblePoints;
    public List<int> pointsPerStar;
    public int boxSize;
    public List<string> snackList;

    public LevelConfigData(
        int levelNumber, float snackQueueDuration, int possibleSnackCount,
        float shiftDuration, int possiblePoints, List<int> pointsPerStar,
        int boxSize, List<string> snackList)
    {
        this.levelNumber = levelNumber;
        this.snackQueueDuration = snackQueueDuration;
        this.possibleSnackCount = possibleSnackCount;
        this.shiftDuration = shiftDuration;
        this.possiblePoints = possiblePoints;
        this.pointsPerStar = pointsPerStar;
        this.boxSize = boxSize;
        this.snackList = snackList;
    }
}
