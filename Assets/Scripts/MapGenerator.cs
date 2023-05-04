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
    private Dictionary<Vector3Int, CubeData> mapCubeData;
    [SerializeField] private GameObject stoneCube;
    [SerializeField] private GameObject grassCube;
    [SerializeField] private int softCubesHeightMax = 10;
    [SerializeField] private GameObject snowCube;
    [Tooltip("Normalized(percent) amount of snow tiles.")]
    [Range(0, 1)]
    [SerializeField] private float snowTrigger;
    private int snowIntHeight;

    private Vector3Int[] neighbourOffsets = {
        new Vector3Int(-1,0,0),
        new Vector3Int(1,0,0),
        new Vector3Int(0,-1,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,0,-1),
        new Vector3Int(0,0,1)
    };
    private void Awake()
    {
        mapGrid = new Column[widthMap, depthMap];
        mapCubeData = new();
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
                var cube = Instantiate(heightOfColumn < actualSoftCubeheStart ? stoneCube : heightOfColumn > snowIntHeight ? snowCube : grassCube, mapPosition, Quaternion.identity);
                CubeData cubeData = new CubeData(mapPosition.ToVec3Int(), cube);
                SaveToMapCubeData(cubeData);
            }                     
        }
    }

    internal void BreakBloack(Vector3Int cubePosition)
    {
        mapCubeData.TryGetValue(cubePosition, out var cubeData);
        Destroy(cubeData.worldCube);
        mapCubeData.Remove(cubePosition);

    }

    private void SaveToMapCubeData(CubeData cubeData)
    {
        mapCubeData.Add(cubeData.position, cubeData);
    }

    internal void PlaceBlock(Vector3 position)
    {
        Vector3Int mapPosition = position.ToVec3Int();
        var cube = Instantiate(grassCube, mapPosition, Quaternion.identity);
        CubeData cubeNew = new CubeData(mapPosition, cube);
        mapCubeData.Add(mapPosition, cubeNew);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
    
    public List<Vector3> GetFreeNeighbourPosition(Vector3Int cubePosition)
    {
        List<Vector3> returnedList = new();

        foreach (Vector3Int offset in neighbourOffsets)
        {
            Vector3Int checkedPosition = cubePosition + offset;
            if(mapCubeData.ContainsKey(checkedPosition)) continue;
            returnedList.Add(checkedPosition);
        }

        return returnedList;
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
public struct CubeData
{
    public Vector3Int position;
    public GameObject worldCube;
    public CubeData(Vector3Int position, GameObject worldCube)
    {
        this.position = position;
        this.worldCube = worldCube;
    }
}
