using UnityEngine;
using UnityEngine.UI; // Required for UI components
using TMPro; // For TextMeshPro support

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

        // Clear existing buttons to prevent duplicates
        foreach (Transform child in worldListContainer)
        {
            Destroy(child.gameObject);
        }

        // Loop through all worlds in the WorldManager
        foreach (WorldData world in worldManager.worlds)
        {
            // Instantiate a button from the prefab
            GameObject buttonObj = Instantiate(worldButtonPrefab, worldListContainer);

            // Get the Button and TMP_Text components from the instantiated button
            Button worldButton = buttonObj.GetComponent<Button>();
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();

            // Get RectTransform of button and text
            RectTransform buttonRectTransform = worldButton.GetComponent<RectTransform>();
            RectTransform textRectTransform = buttonText.GetComponent<RectTransform>();

            //change text size to 100px
            buttonText.fontSize = 20;
            // align text to the left
            buttonText.alignment = TextAlignmentOptions.Left;

            // Set button position and anchor values
            //buttonRectTransform.anchorMin = new Vector2(0, 0);
            //buttonRectTransform.anchorMax = new Vector2(1, 0);
            //buttonRectTransform.anchoredPosition = new Vector2(-300, buttonRectTransform.anchoredPosition.y);

            // Adjust the position of the text within the button
            //textRectTransform.localPosition = new Vector3(0, 0, 0); // Change X, Y, Z values

            // Set the button's text to show the world name and percentage complete
            buttonText.text = $"{world.WorldName} - {world.PercentageComplete}% Complete";

            // If the world is locked, make the button non-interactable and change the color to gray
            worldButton.interactable = !world.IsLocked;
            if (world.IsLocked)
            {
                buttonText.color = Color.gray; // Locked worlds are gray
            }

            // Add button functionality to load the world
            worldButton.onClick.AddListener(() => LoadWorld(world));
        }
    }

    // Method to handle world loading
    void LoadWorld(WorldData world)
    {
        Debug.Log($"Loading world: {world.WorldName}");
        // Add logic to load the world here
    }
}
