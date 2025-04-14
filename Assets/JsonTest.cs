using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "PlayerJsons", "playerData.json");

        Player testPlayer = new Player("TestUser");
        testPlayer.UnlockLevel(1, 2); 
        testPlayer.UnlockLevel(2, 1);

        SaveJson(testPlayer);
    }

    void SaveJson(Player player)
    {
        string json = JsonUtility.ToJson(player, true); 
        File.WriteAllText(filePath, json);
        Debug.Log("JSON Saved to: " + filePath);
    }
}

//      Player Classes  ===========================
[Serializable]
public class Player
{
    public string Username;
    public PlayerProgress Progress;

    public Player(string username)
    {
        Username = username;
        Progress = new PlayerProgress();
    }

    public void UnlockLevel(int worldId, int levelId)
    {
        Progress.UnlockLevel(worldId, levelId);
    }
}

[Serializable]
public class PlayerProgress
{
    public List<WorldProgress> Worlds;

    public PlayerProgress()
    {
        Worlds = new List<WorldProgress> { new WorldProgress(1) };
    }

    public void UnlockLevel(int worldId, int levelId)
    {
        WorldProgress world = Worlds.Find(w => w.WorldId == worldId);
        if (world == null)
        {
            world = new WorldProgress(worldId);
            Worlds.Add(world);
        }
        world.UnlockLevel(levelId);
    }
}

[Serializable]
public class WorldProgress
{
    public int WorldId;
    public List<int> UnlockedLevels;

    public WorldProgress(int worldId)
    {
        WorldId = worldId;
        UnlockedLevels = new List<int>();

        if (worldId == 1)
        {
            UnlockedLevels.Add(1); 
        }
    }

    public void UnlockLevel(int levelId)
    {
        if (!UnlockedLevels.Contains(levelId))
        {
            UnlockedLevels.Add(levelId);
        }
    }
}


