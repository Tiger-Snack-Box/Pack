using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Test Scene")]
    public static void BuildTestScene()
    {
        // Create new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "TestScene";

        // Create GridManager GameObject
        GameObject gridManager = new GameObject("GridManager");
        var gridScript = gridManager.AddComponent<gridScript>();
        // Note: You need to assign tile prefabs manually in the Inspector

        // Create OrderManager GameObject
        GameObject orderManager = new GameObject("OrderManager");
        var orderManagerScript = orderManager.AddComponent<OrderManager>();

        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create EventSystem
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        // Create OrderPanel under Canvas
        GameObject orderPanel = new GameObject("OrderPanel");
        orderPanel.transform.SetParent(canvasGO.transform);
        RectTransform rectTransform = orderPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.8f, 0.5f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.offsetMin = new Vector2(-150, -200);
        rectTransform.offsetMax = new Vector2(0, 200);
        VerticalLayoutGroup layoutGroup = orderPanel.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;

        // Assign orderPanel to OrderManager
        orderManagerScript.orderPanel = orderPanel.transform;

        // Save the scene
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/TestScene.unity");

        EditorUtility.DisplayDialog("Test Scene Created",
            "TestScene.unity has been created and set up with basic components.\n\n✅ IMPORTANT:\nPlease assign your tile prefabs (_tilePrefab1–4) to GridManager in the Inspector, and your OrderUIPrefab to OrderManager.",
            "OK");
    }
}
