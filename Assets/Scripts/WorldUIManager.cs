using UnityEngine;
using UnityEngine.UI; // Required for UI components
using TMPro; // For TextMeshPro support
using UnityEngine.SceneManagement;


public class WorldUIManager : MonoBehaviour
{
    public GameObject worldButtonPrefab;  // The prefab button 
    public Transform worldListContainer;  // Parent object for the list of worlds (Vertical Layout Group)

    private WorldManager worldManager;  // Reference to the WorldManager script

    private void Start()
    {
        if (WorldManager.Instance == null)
        {
            Debug.LogError("WorldManager not found! Make sure it is added to the scene.");
            return;
        }

        worldManager = WorldManager.Instance;
        DisplayWorlds();
    }

    // Method to display worlds as buttons
    void DisplayWorlds()
    {

        foreach (Transform child in worldListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (WorldData world in worldManager.worlds)
        {
            GameObject buttonObj = Instantiate(worldButtonPrefab, worldListContainer);
            Button worldButton = buttonObj.GetComponent<Button>();
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();

            RectTransform buttonRectTransform = worldButton.GetComponent<RectTransform>();
            RectTransform textRectTransform = buttonText.GetComponent<RectTransform>();

            buttonText.fontSize = 20;
            buttonText.alignment = TextAlignmentOptions.Left;

            buttonText.text = $"{world.WorldName} - {world.PercentageComplete}% Complete";

            worldButton.interactable = !world.IsLocked;
            if (world.IsLocked)
            {
                buttonText.color = Color.gray;
            }

            int index = worldManager.worlds.IndexOf(world); 
            worldButton.onClick.AddListener(() =>
            {
                worldManager.SetSelectedWorld(index);  
                LoadWorld(world);                      
            });
        }

    }

    void LoadWorld(WorldData world)
    {
        GameSession.CurrentWorldId = world.WorldId;
        Debug.Log($"Loading World {world.WorldId}: {world.WorldName}");

        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect");
    }

}
