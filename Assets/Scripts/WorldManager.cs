using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }  // Singleton Instance
    public List<WorldData> worlds = new List<WorldData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeWorlds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeWorlds()
    {
        for (int i = 0; i < 15; i++)
        {
            bool locked = (i != 0);  // Unlock first world (i == 0)
            worlds.Add(new WorldData($"World {i + 1}", "0", locked));
        }
    }
}
