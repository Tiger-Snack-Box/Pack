using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    static string filePath = "playerData.json";

    static void Main()
    {
        Console.WriteLine("Starting JSON Test");

        Player testPlayer = new Player("TestUser");
        testPlayer.UnlockLevel(1, 2); 
        testPlayer.UnlockLevel(2, 1); 

        SaveJson(testPlayer);

        Player loadedPlayer = LoadJson();
        Console.WriteLine($"Loaded Player: {loadedPlayer.Username}");

        foreach (var world in loadedPlayer.Progress.Worlds)
        {
            Console.WriteLine($"World {world.WorldId}: Levels Unlocked: {string.Join(", ", world.UnlockedLevels)}");
        }

        Console.ReadKey();
    }

    static void SaveJson(Player player)
    {
        string json = JsonSerializer.Serialize(player, new JsonSerializerOptions { WriteIndented = false });
        File.WriteAllText(filePath, json);
        Console.WriteLine("\nJSON Saved to: " + Path.GetFullPath(filePath));
    }

    static Player LoadJson()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Player>(json);
        }
        else
        {
            return new Player("NewPlayer");
        }
    }
}

//          Player Classes
public class Player
{
    public string Username { get; private set; }
    public PlayerProgress Progress { get; private set; }

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

public class PlayerProgress
{
    public List<WorldProgress> Worlds { get; private set; }

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

public class WorldProgress
{
    public int WorldId { get; private set; }
    public List<int> UnlockedLevels { get; private set; }

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
