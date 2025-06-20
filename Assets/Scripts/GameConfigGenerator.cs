using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameConfigGenerator : MonoBehaviour
{
    private void Start()
    {
        GenerateConfig();
    }

    private void GenerateConfig()
    {
        string folderPath = Path.Combine(Application.dataPath, "GameConfigJson");
        string filePath = Path.Combine(folderPath, "gameconfig.json");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        GameConfigData config = new GameConfigData(new List<WorldConfigData>
        {
            new WorldConfigData(
                1,
                "Humble Beginnings",
                100,
                new List<LevelConfigData>
                {
                    new LevelConfigData(1, 10f, 5, 120f, 3000,
                        new List<int> { 1000, 2000, 3000 },
                        3,
                        new List<string> { "Chocolate", "Candy", "Soda" }
                    ),
                    new LevelConfigData(2, 12f, 6, 130f, 3500,
                        new List<int> { 1200, 2400, 3500 },
                        4,
                        new List<string> { "Chips", "Gummies", "Crackers" }
                    )
                }
            )
        });

        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);

        Debug.Log($"Game config generated at:\n{filePath}");
    }
}
