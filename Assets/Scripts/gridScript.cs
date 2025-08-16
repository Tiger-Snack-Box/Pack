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

    private Color highlightColor = Color.yellow;
    private Color defaultColor = Color.white;

    public delegate void OnTilesFadeCompleteDelegate(Sprite sprite);
    public event OnTilesFadeCompleteDelegate OnTilesFadeComplete;

    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float moveToTargetSpeed = 4f;

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    // Manage active coroutine per tile to prevent conflicts
    private Dictionary<GameObject, Coroutine> activeTileCoroutines = new Dictionary<GameObject, Coroutine>();

    // Swipe data to track current swipes with their matched orders and tiles
    private class SwipeData
    {
        public Order matchedOrder;
        public List<GameObject> tiles;

        public SwipeData(Order order, List<GameObject> tiles)
        {
            this.matchedOrder = order;
            this.tiles = tiles;
        }
    }

    private List<SwipeData> ongoingSwipes = new List<SwipeData>();

    void Start()
    {
        gridMatrix = new List<List<GameObject>>();
        GenerateGrid();
    }

    void Update()
    {
        HandleInput();

        if (isDragging)
        {
            MoveSelectedTiles();
        }
    }

    private Sprite GetRandomItemSprite()
    {
        if (itemSprites.Count == 0) return null;
        int index = Random.Range(0, itemSprites.Count);
        return itemSprites[index];
    }

    [SerializeField] private GameObject gridBackground;

    void GenerateGrid()
    {
        xOffset = (width - 1.5f) / 2.0f;
        yOffset = (height + 4.4f) / 2.0f;

        tileHeight = 0.5f;
        tileWidth = 0f;

        gridMatrix = new List<List<GameObject>>();
        originalPositions.Clear();

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

                    sr.sortingLayerName = "Tiles";
                    sr.sortingOrder = 10;

                    if (sr.sprite != null)
                    {
                        float spriteHeight = sr.sprite.bounds.size.y;
                        if (spriteHeight > 0f)
                        {
                            float scale = tileHeight / spriteHeight;
                            tile.transform.localScale = new Vector3(scale, scale, 1f);

                            if (tileWidth == 0f)
                            {
                                tileWidth = sr.sprite.bounds.size.x * scale;
                            }
                        }
                    }
                }

                row.Add(tile);
                originalPositions[tile] = tile.transform.position;
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

                float scaleX = targetGridWidth * 225 / 100 / bgSize.x;
                float scaleY = targetGridHeight * 225 / 100 / bgSize.y;

                float uniformScale = Mathf.Min(scaleX, scaleY);
                gridBackground.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);

                Vector3 gridCenter = new Vector3((width - 1) / 2f, (height - 1) / 2f, 1f);
                gridCenter.x -= xOffset;
                gridCenter.y -= yOffset + .2f;
                gridBackground.transform.position = gridCenter;

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
            if (!selectedTiles.Contains(tile))
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

        List<GameObject> destroyedTiles = new List<GameObject>();

        foreach (GameObject tile in selectedTiles)
        {
            if (tile == null)
            {
                destroyedTiles.Add(tile);
                continue;
            }

            tile.transform.position = Vector2.Lerp(tile.transform.position, fingerPos, followSpeed * Time.deltaTime);
        }

        foreach (GameObject destroyedTile in destroyedTiles)
        {
            selectedTiles.Remove(destroyedTile);
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

        Order matchedOrder = orderManager.ProcessSwipeGetOrder(swipedSprites);

        List<GameObject> tilesToMove = new List<GameObject>(selectedTiles);
        selectedTiles.Clear();

        if (matchedOrder != null)
        {
            Vector2 targetPosition;
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
                Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, orderTransform.position);
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
                worldPoint.z = 0f;
                targetPosition = worldPoint;
            }
            else
            {
                targetPosition = GetDefaultTargetPosition();
            }

            SwipeData swipeData = new SwipeData(matchedOrder, tilesToMove);
            ongoingSwipes.Add(swipeData);

            // Move each tile individually with coroutine, then fade/destroy after reaching target
            foreach (var tile in tilesToMove)
            {
                StartTileMovementCoroutine(tile, MoveTileToPosition(tile, targetPosition, () =>
                {
                    // On arrival, start fading and destroying tile
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (gridMatrix[x][y] == tile)
                            {
                                StartTileMovementCoroutine(tile, FadeAndDestroyTileCoroutine(tile, x, y, swipeData));
                                return;
                            }
                        }
                    }
                }));
            }
        }
        else
        {
            // Incompatible swipe - move tiles back to original positions individually
            foreach (var tile in tilesToMove)
            {
                Vector3 originalPos = originalPositions.ContainsKey(tile) ? originalPositions[tile] : tile.transform.position;

                // Reset tile color to default immediately (or could do on finish)
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = defaultColor;
                }

                StartTileMovementCoroutine(tile, MoveTileToPosition(tile, originalPos));
            }
        }
    }

    private void StartTileMovementCoroutine(GameObject tile, IEnumerator coroutine)
    {
        if (tile == null) return;

        // Cancel any existing coroutine on this tile
        if (activeTileCoroutines.TryGetValue(tile, out Coroutine existingCoroutine))
        {
            StopCoroutine(existingCoroutine);
        }

        Coroutine newCoroutine = StartCoroutine(CoroutineWrapper(tile, coroutine));
        activeTileCoroutines[tile] = newCoroutine;
    }

    private IEnumerator CoroutineWrapper(GameObject tile, IEnumerator coroutine)
    {
        yield return coroutine;
        activeTileCoroutines.Remove(tile);
    }

    private IEnumerator MoveTileToPosition(GameObject tile, Vector3 targetPosition, System.Action onComplete = null)
    {
        if (tile == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        while (Vector3.Distance(tile.transform.position, targetPosition) > 0.05f)
        {
            tile.transform.position = Vector3.Lerp(tile.transform.position, targetPosition, moveToTargetSpeed * Time.deltaTime);
            yield return null;
        }

        tile.transform.position = targetPosition;

        onComplete?.Invoke();
    }

    private IEnumerator FadeAndDestroyTileCoroutine(GameObject tile, int x, int y, SwipeData swipeData)
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
        }

        Destroy(tile);
        gridMatrix[x][y] = CreateTileAt(x, y);

        swipeData.tiles.Remove(tile);

        if (swipeData.tiles.Count == 0)
        {
            if (swipeData.matchedOrder != null)
            {
                OnTilesFadeComplete?.Invoke(swipeData.matchedOrder.sprite);
            }

            ongoingSwipes.Remove(swipeData);
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

        originalPositions[newTile] = pos;

        return newTile;
    }

    Vector2 GetDefaultTargetPosition()
    {
        float targetX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.8f, 0)).x;
        float targetY = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.2f)).y;
        return new Vector2(targetX, targetY);
    }
}
