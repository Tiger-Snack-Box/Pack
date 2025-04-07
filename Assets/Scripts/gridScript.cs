using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

public class gridScript : MonoBehaviour
{
    [SerializeField] private int width, height; //serialize field, modifiable scale editor, 
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _tilePrefab2;
    [SerializeField] private GameObject _tilePrefab3;
    [SerializeField] private GameObject _tilePrefab4;
    private List<List<GameObject>> gridMatrix;
    
    void Start()
    {
        gridMatrix = new List<List<GameObject>>(); 
        generateGrid();
        removeTile(3, 3);
        changeTile(_tilePrefab2, 2, 2);
    }
    void generateGrid()
    {
        float xOffset = (width - 1) / 2.0f;
        float yOffset = (height - 1) / 2.0f;

        for (int x = 0; x < width; x++)
        {
            List<GameObject> list = new List<GameObject>();
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = new Vector3(x - xOffset, y - yOffset, 0); // Centered position
                GameObject spawnedTile = Instantiate(_tilePrefab, spawnPosition, Quaternion.identity, gameObject.transform);
                spawnedTile.name = $"Tile {x} {y}";
                list.Add(spawnedTile);
            }
            gridMatrix.Add(list);
        }
    }
    void removeTile(int x, int y)
    {
        Destroy(gridMatrix[x][y]);
    }
    void changeTile(GameObject tileName, int x, int y)
    {
        GameObject spawnedTile = Instantiate(tileName, new Vector3(x, y), Quaternion.identity, gameObject.transform);
        spawnedTile.name = $"Tile {x} {y}";
        gridMatrix[x][y] = spawnedTile;
    }
}

public class GridScript : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private GameObject tilePrefab;
    private List<List<GameObject>> gridMatrix;

    private bool isDragging = false;
    private HashSet<GameObject> selectedTiles = new HashSet<GameObject>();
    private List<GameObject> movingTiles = new List<GameObject>(); 

    private Color highlightColor = Color.yellow;
    private Color defaultColor = Color.white;

    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float moveToTargetSpeed = 4f;
    private Vector2 targetPosition;

    void Start()
    {
        gridMatrix = new List<List<GameObject>>();
        GenerateGrid();
        UpdateTargetPosition();
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

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            List<GameObject> row = new List<GameObject>();
            for (int y = 0; y < height; y++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                tile.name = $"Tile {x} {y}";
                tile.AddComponent<BoxCollider2D>();

                SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = defaultColor;
                }
                row.Add(tile);

            }
            gridMatrix.Add(row);

        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            TrySelectTile(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StartMovingToTarget();
        }

        if (isDragging)
        {
            TrySelectTile(Input.mousePosition);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                TrySelectTile(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (isDragging)
                {
                    TrySelectTile(touch.position);
                }

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
                StartMovingToTarget();
            }
        }
    }

    void TrySelectTile(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject tile = hit.collider.gameObject;

            if (!selectedTiles.Contains(tile) && !movingTiles.Contains(tile))
            {
                selectedTiles.Add(tile);
                HighlightTile(tile);
            }

        }
    }

    void HighlightTile(GameObject tile)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    void MoveSelectedTiles()
    {
        Vector2 fingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (GameObject tile in selectedTiles)
        {
            tile.transform.position = Vector2.Lerp(tile.transform.position, fingerPos, followSpeed * Time.deltaTime);
        }
    }

    void StartMovingToTarget()
    {
        movingTiles.AddRange(selectedTiles);
        selectedTiles.Clear(); 
    }

    void MoveTilesToTarget()
    {
        List<GameObject> finishedMoving = new List<GameObject>();

        foreach (GameObject tile in movingTiles)
        {
            tile.transform.position = Vector2.Lerp(tile.transform.position, targetPosition, moveToTargetSpeed * Time.deltaTime);

            if (Vector2.Distance(tile.transform.position, targetPosition) < 0.1f)
            {
                finishedMoving.Add(tile);
            }

        }

        foreach (GameObject tile in finishedMoving)
        {
            movingTiles.Remove(tile);
            StartCoroutine(FadeAndDestroyTile(tile));
        }

    }

    System.Collections.IEnumerator FadeAndDestroyTile(GameObject tile)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float duration = 0.5f;  
            float elapsedTime = 0;
            Color startColor = spriteRenderer.color;

            while (elapsedTime < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(tile); 
        }

    }

    void UpdateTargetPosition()
    {
        float targetX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.8f, 0)).x;
        float targetY = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.2f)).y;
        targetPosition = new Vector2(targetX, targetY);
    }

}