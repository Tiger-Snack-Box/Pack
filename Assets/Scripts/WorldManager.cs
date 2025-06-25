using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    public List<WorldData> worlds = new List<WorldData>();

    private string playerDataPath;
    private string gameConfigPath;

    public int SelectedWorldIndex { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Cross-platform paths
            playerDataPath = Path.Combine(Application.persistentDataPath, "playerData.json");
            gameConfigPath = Path.Combine(Application.streamingAssetsPath, "gameconfig.json");

            StartCoroutine(InitializeWorlds());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator InitializeWorlds()
    {
        string json = null;

#if UNITY_ANDROID && !UNITY_EDITOR
    UnityWebRequest request = UnityWebRequest.Get(gameConfigPath);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        json = request.downloadHandler.text;
    }
    else
    {
        Debug.LogError("Failed to load gameconfig.json: " + request.error);
        yield break;
    }
#else
        if (File.Exists(gameConfigPath))
        {
            json = File.ReadAllText(gameConfigPath);
        }
        else
        {
            Debug.LogError("gameconfig.json not found at: " + gameConfigPath);
            yield break;
        }
#endif

        GameConfigData config = JsonUtility.FromJson<GameConfigData>(json);
        foreach (WorldConfigData configWorld in config.worlds)
        {
            bool isLocked = true;
            worlds.Add(new WorldData(configWorld.worldNumber, configWorld.worldName, "0%", isLocked));
        }

        // **Unlock first world by default!**
        if (worlds.Count > 0)
        {
            worlds[0].IsLocked = false;
        }

        Debug.Log($"Loaded {worlds.Count} worlds from gameconfig");

        LoadPlayerProgress(); // This might unlock more worlds if player progress exists
    }


    private void LoadPlayerProgress()
    {
        // Update playerDataPath to persistentDataPath location
        playerDataPath = Path.Combine(Application.persistentDataPath, "playerData.json");

        if (File.Exists(playerDataPath))
        {
            string json = File.ReadAllText(playerDataPath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            foreach (WorldProgress world in playerData.Progress.Worlds)
            {
                int worldIndex = world.WorldId - 1;
                if (worldIndex >= 0 && worldIndex < worlds.Count)
                {
                    worlds[worldIndex].IsLocked = false;
                }
            }

            Debug.Log("Player progress loaded successfully from: " + playerDataPath);
        }
        else
        {
            Debug.LogWarning("No playerData.json found at: " + playerDataPath + ". Creating default save.");
            CreateDefaultSave();
        }
    }

    private void CreateDefaultSave()
    {
        PlayerData defaultPlayerData = new PlayerData
        {
            Username = "Player",
            Progress = new PlayerProgress() // Make sure this unlocks the first world & level by default
        };

        string json = JsonUtility.ToJson(defaultPlayerData, true);
        File.WriteAllText(playerDataPath, json);
        Debug.Log("Default player data created at: " + playerDataPath);

        // Also unlock the first world in the current session
        if (worlds.Count > 0)
        {
            worlds[0].IsLocked = false;
        }
    }

    public void SetSelectedWorld(int index)
    {
        SelectedWorldIndex = index;
    }

    public void SavePlayerProgress()
    {
        PlayerData playerData = new PlayerData();
        playerData.Username = "Player";
        playerData.Progress = new PlayerProgress();
        playerData.Progress.Worlds = new List<WorldProgress>();

        // Only include worlds the player actually has progress in
        foreach (var world in worlds)
        {
            if (!world.IsLocked)
            {
                // Attempt to find existing world progress (e.g., only World 1 has Level 1 unlocked)
                WorldProgress existingProgress = new WorldProgress(world.WorldId);

                // You can later track actual level progress, for now only save if the world is World 1
                if (world.WorldId == 1)
                {
                    existingProgress.UnlockedLevels = new List<int> { 1 }; // Only level 1 in world 1
                }

                playerData.Progress.Worlds.Add(existingProgress);
            }
        }

        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(playerDataPath, json);
        Debug.Log("Player progress saved.");
    }

}