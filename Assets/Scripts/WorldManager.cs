using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    public List<WorldData> worlds = new List<WorldData>();
    private string filePath;
    private string gameConfigPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.dataPath, "PlayerJsons", "playerData.json");
            gameConfigPath = Path.Combine(Application.dataPath, "GameConfigJson", "gameconfig.json");
            InitializeWorlds();
            LoadPlayerProgress(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeWorlds()
    {
        if (File.Exists(gameConfigPath))
        {
            string json = File.ReadAllText(gameConfigPath);
            GameConfigData config = JsonUtility.FromJson<GameConfigData>(json);

            foreach (WorldConfigData configWorld in config.worlds)
            {
                bool isLocked = true;

                worlds.Add(new WorldData(configWorld.worldNumber,
                    configWorld.worldName, "0%", isLocked));
            }
            Debug.Log($"Loaded {worlds.Count} worlds from gameconfig");
        } else
        {
            Debug.LogError($"Game config not found");
        }
    }

    private void LoadPlayerProgress()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            foreach (WorldProgress world in playerData.Progress.Worlds)
            {
                int worldIndex = world.WorldId - 1;
                if (worldIndex >= 0 && worldIndex < worlds.Count)
                {
                    worlds[worldIndex].IsLocked = false; 
                }
            }

            Debug.Log("Player progress loaded successfully");
        }
        else
        {
            Debug.LogWarning("No save file found");
        }
    }

    public int SelectedWorldIndex { get; private set; }

    public void SetSelectedWorld(int index)
    {
        SelectedWorldIndex = index;
    }

}


[System.Serializable]
public class PlayerData
{
    public string Username;
    public PlayerProgress Progress;
}


