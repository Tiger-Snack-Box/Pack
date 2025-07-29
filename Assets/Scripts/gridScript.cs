using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System

public class GridScript : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] public List<Sprite> itemSprites;
    [SerializeField] private OrderManager orderManager;
    private float xOffset, yOffset;
    private float tileHeight = 0.5f;
    private float tileWidth;
    private List<List<GameObject>> gridMatrix;

    private bool isDragging = false;
    private HashSet<GameObject> selectedTiles = new HashSet<GameObject>();
    private List<GameObject> movingTiles = new List<GameObject>();
    private Vector2 currentTargetPosition;
    private Order matchedOrder = null;

    private Color highlightColor = Color.yellow;
    private Color defaultColor = Color.white;

    public delegate void OnTilesFadeCompleteDelegate(Sprite sprite);
    public event OnTilesFadeCompleteDelegate OnTilesFadeComplete;

    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float moveToTargetSpeed = 4f;

    void Start()
    {
        gridMatrix = new List<List<GameObject>>();
        GenerateGrid();
        currentTargetPosition = GetDefaultTargetPosition();
    }

    void Update()
    {
        HandleInput();

        if (isDragging)
        {
            MoveSelectedTiles();
        }

        if (movingTiles.Count > 0)
        {
            MoveTilesToTarget();
        }
    }

    private Sprite GetRandomItemSprite()
    {
        if (itemSprites.Count == 0) return null;
        int index = Random.Range(0, itemSprites.Count);
        return itemSprites[index];
    }

    [SerializeField] private GameObject gridBackground; // Assign your grid background GameObject here in inspector

    void GenerateGrid()
    {
        xOffset = (width - 1.5f) / 2.0f;
        yOffset = (height + 4.4f) / 2.0f;

        tileHeight = 0.5f;
        tileWidth = 0f;

        gridMatrix = new List<List<GameObject>>();

        // Instantiate tiles and calculate tileWidth based on first tile sprite scale
        for (int x = 0; x < width; x++)
        {
            List<GameObject> row = new List<GameObject>();
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = new Vector3(x - xOffset, y - yOffset, 0);
                GameObject tile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform);
                tile.name = $"Tile {x} {y}";
                tile.AddComponent<BoxCollider2D>();

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = GetRandomItemSprite();
                    sr.color = defaultColor;

                    // Set sorting layer and order so tile appears above background
                    sr.sortingLayerName = "Tiles";
                    sr.sortingOrder = 10;

                    if (sr.sprite != null)
                    {
                        float spriteHeight = sr.sprite.bounds.size.y;
                        if (spriteHeight > 0f)
                        {
                            float scale = tileHeight / spriteHeight;
                            tile.transform.localScale = new Vector3(scale, scale, 1f);

                            // Calculate tileWidth based on scaled sprite width only once (on first tile)
                            if (tileWidth == 0f)
                            {
                                tileWidth = sr.sprite.bounds.size.x * scale;
                            }
                        }
                    }
                }
                row.Add(tile);
            }
            gridMatrix.Add(row);
        }

        if (gridBackground != null)
        {
            SpriteRenderer bgRenderer = gridBackground.GetComponent<SpriteRenderer>();
            if (bgRenderer != null && bgRenderer.sprite != null)
            {
                Vector2 bgSize = bgRenderer.sprite.bounds.size;

                float targetGridWidth = tileWidth * width;
                float targetGridHeight = tileHeight * height;

                // Calculate scale factors
                float scaleX = targetGridWidth * 225/100/ bgSize.x;
                float scaleY = targetGridHeight * 225/100/ bgSize.y;

                // Apply uniform scale
                float uniformScale = Mathf.Min(scaleX, scaleY);
                gridBackground.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);

                // Correct position: center of the tile grid
                Vector3 gridCenter = new Vector3((width - 1) / 2f, (height - 1) / 2f, 1f);
                gridCenter.x -= xOffset;
                gridCenter.y -= yOffset+.2f;
                gridBackground.transform.position = gridCenter;

                // Ensure background renders behind
                bgRenderer.sortingLayerName = "Background";
                bgRenderer.sortingOrder = 0;
            }
        }
    }

    void HandleInput()
    {
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isDragging = true;
                TrySelectTile(Mouse.current.position.ReadValue());
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                StartMovingToTarget();
            }

            if (isDragging)
            {
                TrySelectTile(Mouse.current.position.ReadValue());
            }
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                isDragging = true;
                TrySelectTile(touch.position.ReadValue());
            }
            else if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved ||
                     touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                if (isDragging)
                {
                    TrySelectTile(touch.position.ReadValue());
                }
            }
            else if (touch.press.wasReleasedThisFrame)
            {
                isDragging = false;
                StartMovingToTarget();
            }
        }
    }

    void TrySelectTile(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        float selectionRadius = 0.3f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, selectionRadius);
        foreach (Collider2D hit in hits)
        {
            GameObject tile = hit.gameObject;
            if (!selectedTiles.Contains(tile) && !movingTiles.Contains(tile))
            {
                selectedTiles.Add(tile);
                HighlightTile(tile);
            }
        }
    }

    void HighlightTile(GameObject tile)
    {
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = highlightColor;
        }
    }

    void MoveSelectedTiles()
    {
        Vector2 fingerPos = Vector2.zero;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            fingerPos = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
        }
        else if (Mouse.current != null)
        {
            fingerPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        foreach (GameObject tile in selectedTiles)
        {
            tile.transform.position = Vector2.Lerp(tile.transform.position, fingerPos, followSpeed * Time.deltaTime);
        }
    }

    void StartMovingToTarget()
    {
        if (selectedTiles.Count == 0) return;

        List<Sprite> swipedSprites = new List<Sprite>();
        foreach (GameObject tile in selectedTiles)
        {
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
                swipedSprites.Add(sr.sprite);
        }

        // Get the order matched for this swipe, but do NOT remove yet
        matchedOrder = orderManager.ProcessSwipeGetOrder(swipedSprites);

        if (matchedOrder != null)
        {
            // Find the OrderUI transform for matchedOrder
            Transform orderTransform = null;
            foreach (Transform child in orderManager.orderPanel)
            {
                OrderUI ui = child.GetComponent<OrderUI>();
                if (ui != null && ui.order == matchedOrder)
                {
                    orderTransform = child;
                    break;
                }
            }

            if (orderTransform != null)
            {
                // Convert UI position (Canvas space) to world space
                Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, orderTransform.position);
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
                worldPoint.z = 0f;
                currentTargetPosition = worldPoint;
            }
            else
            {
                // Fallback default if UI not found
                currentTargetPosition = GetDefaultTargetPosition();
            }
        }
        else
        {
            // No compatible order, fly to default position
            currentTargetPosition = GetDefaultTargetPosition();
        }

        movingTiles.AddRange(selectedTiles);
        selectedTiles.Clear();
    }

    void MoveTilesToTarget()
    {
        List<GameObject> finishedMoving = new List<GameObject>();

        foreach (GameObject tile in movingTiles)
        {
            tile.transform.position = Vector2.Lerp(tile.transform.position, currentTargetPosition, moveToTargetSpeed * Time.deltaTime);

            if (Vector2.Distance(tile.transform.position, currentTargetPosition) < 0.1f)
            {
                finishedMoving.Add(tile);
            }
        }

        foreach (GameObject tile in finishedMoving)
        {
            movingTiles.Remove(tile);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (gridMatrix[x][y] == tile)
                    {
                        StartCoroutine(FadeAndDestroyTile(tile, x, y));
                    }
                }
            }
        }
    }

    GameObject CreateTileAt(int x, int y)
    {
        Vector3 pos = new Vector3(x - xOffset, y - yOffset, 0);

        GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
        newTile.name = $"Tile {x} {y}";
        newTile.AddComponent<BoxCollider2D>();

        SpriteRenderer sr = newTile.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = GetRandomItemSprite();
            sr.color = defaultColor;

            // --- Quick fix: Set sorting layer and order for new tiles ---
            sr.sortingLayerName = "Tiles";
            sr.sortingOrder = 10;

            if (sr.sprite != null)
            {
                float targetHeight = 0.5f;
                float spriteHeight = sr.sprite.bounds.size.y;

                if (spriteHeight > 0f)
                {
                    float scale = targetHeight / spriteHeight;
                    newTile.transform.localScale = new Vector3(scale, scale, 1f);
                }
            }
        }

        return newTile;
    }

    IEnumerator FadeAndDestroyTile(GameObject tile, int x, int y)
    {
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float duration = 0.5f;
            float elapsed = 0;
            Color startColor = sr.color;

            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(tile);
            gridMatrix[x][y] = CreateTileAt(x, y);
        }

        // Check if all movingTiles have finished fading
        if (movingTiles.Count == 0 && matchedOrder != null)
        {
            OnTilesFadeComplete?.Invoke(matchedOrder.sprite);
            matchedOrder = null;
        }
    }

    Vector2 GetDefaultTargetPosition()
    {
        float targetX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.8f, 0)).x;
        float targetY = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.2f)).y;
        return new Vector2(targetX, targetY);
    }
}
