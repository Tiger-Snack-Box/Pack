using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");

        Player testPlayer = new Player("TestUser");
        testPlayer.UnlockLevel(1, 2); 
        testPlayer.UnlockLevel(2, 1);

        SaveJson(testPlayer);
    }

    void SaveJson(Player player)
    {
        string json = JsonUtility.ToJson(player, true);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("JSON Saved to: " + filePath);
        }
        catch (IOException ex)
        {
            Debug.LogError("Failed to save JSON: " + ex.Message);
        }
    }
}

