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
        for (int x = 0; x < width; x++)
        {
            List<GameObject> list = new List<GameObject>(); 
            for(int y = 0; y < height; y++)
            {
                GameObject spawnedTile = Instantiate(_tilePrefab, new Vector3(x,y), Quaternion.identity, gameObject.transform);
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
