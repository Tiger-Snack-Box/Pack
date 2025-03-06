using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditorInternal;

public class LevelManager : MonoBehaviour
{
    public Transform buttonContainer;   // to be assigned
    public Button levelButtonPrefab;    // to be assigned

    private List<LevelData> levels = new List<LevelData>();

    private void Start()
    {
        GenerateLevels();
        CreateLevelButtons();
    }

    void GenerateLevels()
    {
        for(int i = 1; i <= 15; i++)
        {
            bool isLocked = (i != 1);
            levels.Add(new LevelData(i, isLocked));
        }
    }

    void CreateLevelButtons()
    {
        foreach (LevelData level in levels)
        {
            Button button = Instantiate(levelButtonPrefab, buttonContainer);
            button.GetComponentInChildren<Text>().text = $"Level {level.levelNumber}";
            button.interactable = !level.isLocked;

            // to be done: click event
            int levelNum = level.levelNumber;
            // here:
        }
    }

    // to be done: load level func --> goes with click event in CreateLevelButtons()


}
