using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public string Username;
    public PlayerProgress Progress;

    public Player(string username)
    {
        Username = username;
        Progress = new PlayerProgress(); // unlocks world 1, level 1 by default
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
        Worlds = new List<WorldProgress> { new WorldProgress(1) }; // world 1 unlocked, level 1 unlocked by WorldProgress constructor
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
            UnlockedLevels.Add(1); // first level unlocked by default for first world
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

[Serializable]
public class PlayerData
{
    public string Username;
    public PlayerProgress Progress;

    public PlayerData()
    {
        Username = "DefaultPlayer";
        Progress = new PlayerProgress(); // unlocks first world and level by default
    }
}
