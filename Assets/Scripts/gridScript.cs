using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

public class gridScript : MonoBehaviour
{
    [SerializeField] private int width, height; //serialize field, modifiable scale editor, 
    [SerializeField] private GameObject tilePrefab;
    private List<List<GameObject>> gridMatrix;

    private bool isDragging = false;
    private HashSet<GameObject> selectedTiles = new HashSet<GameObject>();

    private Color highlightColor = Color.yellow;
    private Color defaultColor = Color.white;

    [SerializeField] private float followSpeed = 5f;


    void Start()
    {
        gridMatrix = new List<List<GameObject>>(); 
        generateGrid();
        //removeTile(3, 3);
        //changeTile(_tilePrefab2, 2, 2);
    }
    void Update()
    {
        handleInput();
        if (isDragging)
        {
            moveSelectedTiles();
        }
    }


    void generateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            List<GameObject> row = new List<GameObject>(); 
            for(int y = 0; y < height; y++)
            {
                GameObject spawnedTile = Instantiate(tilePrefab, new Vector3(x,y,0), Quaternion.identity, gameObject.transform);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.AddComponent<BoxCollider2D>();

                SpriteRenderer spriteRenderer = spawnedTile.GetComponent<SpriteRenderer>();
                if(spriteRenderer != null)
                {
                    spriteRenderer.color = defaultColor;
                }
                row.Add(spawnedTile);
            }
            gridMatrix.Add(row);
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

    void handleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            trySelectTile(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            resetTiles();
        }

        if (isDragging)
        {
            trySelectTile(Input.mousePosition);
        }

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                trySelectTile(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if(isDragging)
                {
                    trySelectTile(touch.position);
                }
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
                resetTiles();
            }
        }


    }

    void trySelectTile(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if(hit.collider != null)
        {
            GameObject tile = hit.collider.gameObject;
            if(!selectedTiles.Contains(tile))
            {
                selectedTiles.Add(tile);
                highlightTile(tile);
            }
        }
    }

    void highlightTile(GameObject tile)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    void resetTiles()
    {
        foreach (GameObject tile in selectedTiles)
        {
            StartCoroutine(FadeTileColor(tile));
        }
        selectedTiles.Clear();
    }

    System.Collections.IEnumerator FadeTileColor(GameObject tile)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float duration = 0.3f;
            float elapsedTime = 0;
            Color startColor = spriteRenderer.color;

            while (elapsedTime < duration)
            {
                spriteRenderer.color = Color.Lerp(startColor, defaultColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = defaultColor; 
        }
    }

    void moveSelectedTiles()
    {
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (GameObject tile in selectedTiles)
        {
            tile.transform.position = Vector2.Lerp(tile.transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }

}
