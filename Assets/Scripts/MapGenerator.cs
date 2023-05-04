using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int widthMap;
    [SerializeField] private int depthMap;
    [SerializeField] private int heightMap;
    [SerializeField] private float multiplier;
    [SerializeField] private Column[,] mapGrid;
    [SerializeField] private GameObject stoneCube;
    [SerializeField] private GameObject grassCube;
    [SerializeField] private int softCubesHeightMax = 10;
    [SerializeField] private GameObject snowCube;
    [Tooltip("Normalized(percent) amount of snow tiles.")]
    [Range(0, 1)]
    [SerializeField] private float snowTrigger;
    private int snowIntHeight;
    private void Awake()
    {
        mapGrid = new Column[widthMap, depthMap];
        snowIntHeight = (heightMap * snowTrigger).ToInt();
    }
    void Start()
    {
        float perlinOffset = UnityEngine.Random.Range(10000, 100000);
        for (int x = 0; x < widthMap; x++)
        {
            for (int z = 0; z < depthMap; z++)
            {
                int cubeHeight = (Mathf.PerlinNoise(x * multiplier, z * multiplier) * heightMap).ToInt();
                int softCubesHeight = (Mathf.PerlinNoise(x * multiplier + perlinOffset, z * multiplier + perlinOffset) * softCubesHeightMax).ToInt();
                mapGrid[x, z] = new Column(new Vector2Int(x, z), cubeHeight, softCubesHeight);
            }
        }

        foreach (Column actualColumn in mapGrid)
        {
            int actualSoftCubeheStart = actualColumn.height - actualColumn.softCubesHeight;
            for (int heightOfColumn = 0; heightOfColumn < actualColumn.height; heightOfColumn++)
            {
                Vector3 mapPosition = new(actualColumn.position.x, heightOfColumn, actualColumn.position.y);                
                Instantiate(heightOfColumn < actualSoftCubeheStart ? stoneCube : heightOfColumn > snowIntHeight ? snowCube : grassCube, mapPosition, Quaternion.identity);
            }                     
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
public struct Column
{
    public Vector2Int position;
    public int height;
    public int softCubesHeight;
    public Column(Vector2Int position, int height, int softCubesHeight)
    {
        this.position = position;
        this.height = height;
        this.softCubesHeight = softCubesHeight;
    }
}
