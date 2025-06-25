using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LevelManager : MonoBehaviour
{
    public Transform buttonContainer;   // to be assigned
    public Button levelButtonPrefab;    // to be assigned

    private List<LevelData> levels = new List<LevelData>();

    private void Start()
    {
        StartCoroutine(LoadAndDisplayLevels());
    }

    private IEnumerator LoadAndDisplayLevels()
    {
        yield return StartCoroutine(LoadLevelData());
        CreateLevelButtons();
    }

    public IEnumerator LoadLevelData()
    {
        int selectedWorldIndex = WorldManager.Instance.SelectedWorldIndex;
        int selectedWorldNumber = selectedWorldIndex + 1;

        string configPath = Path.Combine(Application.streamingAssetsPath, "gameconfig.json");
        string playerDataPath = Path.Combine(Application.persistentDataPath, "playerData.json");

        string configJson = null;
        string playerJson = null;

#if UNITY_ANDROID && !UNITY_EDITOR
        // Android: Use UnityWebRequest to load JSON files from StreamingAssets
        UnityWebRequest configRequest = UnityWebRequest.Get(configPath);
        yield return configRequest.SendWebRequest();
        if (configRequest.result == UnityWebRequest.Result.Success)
        {
            configJson = configRequest.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Failed to load gameconfig.json: " + configRequest.error);
            yield break;
        }

        if (File.Exists(playerDataPath))
        {
            playerJson = File.ReadAllText(playerDataPath);
        }
        else
        {
            Debug.LogWarning("playerData.json not found at: " + playerDataPath);
        }
#else
        // Non-Android (Editor/Desktop): Use File.ReadAllText
        if (File.Exists(configPath))
        {
            configJson = File.ReadAllText(configPath);
        }
        else
        {
            Debug.LogError("gameconfig.json not found at: " + configPath);
            yield break;
        }

        if (File.Exists(playerDataPath))
        {
            playerJson = File.ReadAllText(playerDataPath);
        }
        else
        {
            Debug.LogWarning("playerData.json not found at: " + playerDataPath);
        }
#endif

        // Parse player data (optional)
        List<int> unlockedLevels = new List<int>();
        if (!string.IsNullOrEmpty(playerJson))
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(playerJson);
            var worldProgress = playerData.Progress.Worlds
                .FirstOrDefault(w => w.WorldId == selectedWorldNumber);

            if (worldProgress != null && worldProgress.UnlockedLevels != null)
            {
                unlockedLevels = worldProgress.UnlockedLevels;
            }
        }

        // Parse config data
        GameConfigData config = JsonUtility.FromJson<GameConfigData>(configJson);
        WorldConfigData selectedWorld = config.worlds
            .FirstOrDefault(w => w.worldNumber == selectedWorldNumber);

        if (selectedWorld != null)
        {
            levels.Clear(); // Clear existing levels if reloading
            foreach (LevelConfigData level in selectedWorld.levels)
            {
                bool isLocked = !unlockedLevels.Contains(level.levelNumber);
                levels.Add(new LevelData(level.levelNumber, isLocked));
            }

            Debug.Log($"Loaded {levels.Count} levels for World {selectedWorldNumber}");
        }
        else
        {
            Debug.LogError($"World {selectedWorldNumber} not found in GameConfig.");
        }
    }

    void CreateLevelButtons()
    {
        Color lockedColor = new Color32(242, 112, 89, 255);

        foreach (LevelData level in levels)
        {
            Button button = Instantiate(levelButtonPrefab, buttonContainer);
            Image buttonBackground = button.GetComponent<Image>();

            Text levelText = button.transform.Find("Level").GetComponent<Text>();
            Text levelNumberText = button.transform.Find("LevelText").GetComponent<Text>();
            Text lockedText = button.transform.Find("Locked").GetComponent<Text>();
            Transform lockImage = button.transform.Find("LockImage");

            if (level.isLocked)
            {
                if (lockImage != null) lockImage.gameObject.SetActive(true);
                if (lockedText != null) lockedText.gameObject.SetActive(true);
                if (levelText != null) levelText.gameObject.SetActive(false);
                if (levelNumberText != null) levelNumberText.gameObject.SetActive(false);
                if (buttonBackground != null) buttonBackground.color = lockedColor;

                foreach (var star in button.GetComponentsInChildren<Image>(true)
                                           .Where(img => img.gameObject.CompareTag("Star")))
                {
                    star.gameObject.SetActive(false);
                }

                button.interactable = false;
            }
            else
            {
                if (lockImage != null) lockImage.gameObject.SetActive(false);
                if (lockedText != null) lockedText.gameObject.SetActive(false);
                if (levelText != null) levelText.gameObject.SetActive(true);
                if (levelNumberText != null)
                {
                    levelNumberText.gameObject.SetActive(true);
                    levelNumberText.text = level.levelNumber.ToString();
                }

                foreach (var star in button.GetComponentsInChildren<Image>(true)
                                           .Where(img => img.gameObject.CompareTag("Star")))
                {
                    star.gameObject.SetActive(true);
                }

                SetStarRating(button, level.rating);

                int levelNum = level.levelNumber;
                // button.onClick.AddListener(() => LoadLevel(levelNum));
            }
        }
    }

    void SetStarRating(Button button, int rating)
    {
        Color yellow = Color.yellow;
        Color grey = Color.grey;

        Image[] stars = button.GetComponentsInChildren<Image>(true)
                          .Where(img => img.gameObject.CompareTag("Star"))
                          .ToArray();

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = (i < rating) ? yellow : grey;
        }
    }
}
