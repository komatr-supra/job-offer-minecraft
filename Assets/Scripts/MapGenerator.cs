using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //todo 2d grid biome generator -> near create new terrain -> assembly new terrain(async?)
    [SerializeField] private int widthMap;
    [SerializeField] private int depthMap;
    [SerializeField] private int heightMap;
    [SerializeField] private BiomesSO biome;
    [SerializeField] private Column[,] mapGrid;
    [SerializeField] private BlocksSO[] usedBlockDatabase;
    private Dictionary<Vector3Int, CubeData> mapCubeData;
    [SerializeField] private int snowIntHeight;
    [SerializeField] private GameObject blockPrefab;
    //todo grid system
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
        usedBlockDatabase = new BlocksSO[]{biome.softBlock, biome.hardBlock, biome.topBlock};
    }
    void Start()
    {
        float perlinOffset = UnityEngine.Random.Range(10000, 100000);
        for (int x = 0; x < widthMap; x++)
        {
            for (int z = 0; z < depthMap; z++)
            {
                int cubeHeight = (Mathf.PerlinNoise(x * biome.multiplier, z * biome.multiplier) * heightMap).ToInt();
                int softCubesHeight = (Mathf.PerlinNoise(x * biome.multiplier + perlinOffset, z * biome.multiplier + perlinOffset) * biome.softCubesHeightMax).ToInt();
                mapGrid[x, z] = new Column(new Vector2Int(x, z), cubeHeight, softCubesHeight);

            }
        }
        //todo make it as active chunk loader(world manager?)
        foreach (Column actualColumn in mapGrid)
        {
            int actualSoftCubeheStart = actualColumn.height - actualColumn.softCubesHeight;
            for (int heightOfBuildColumn = 0; heightOfBuildColumn < actualColumn.height; heightOfBuildColumn++)
            {
                Vector3 mapPosition = new(actualColumn.position.x, heightOfBuildColumn, actualColumn.position.y);
                Blocks block = heightOfBuildColumn < actualSoftCubeheStart ? Blocks.Hard : heightOfBuildColumn < snowIntHeight ? Blocks.Soft : Blocks.Top;
                CreateBlock(mapPosition, block);
            }
        }
    }
    //todo world manager
    private void CreateBlock(Vector3 mapPosition, Blocks block)
    {
        var cube = Instantiate(blockPrefab, mapPosition, Quaternion.identity);
        //block is empty, dont need data, data is in grid system
        ushort blockDatabaseIndex = (ushort)block;
        SetBlockVisual(cube, blockDatabaseIndex);
        CubeData cubeData = new CubeData(mapPosition.ToVec3Int(), cube, blockDatabaseIndex);
        SaveToMapCubeData(cubeData);
    }

    private void SetBlockVisual(GameObject cube, ushort blockDatabaseIndex)
    {
        cube.GetComponent<MeshRenderer>().material = usedBlockDatabase[blockDatabaseIndex].material;
    }
    //todo drop system
    public void BreakBlock(Vector3Int cubePosition)
    {
        if(cubePosition.y == 0) return;
        mapCubeData.TryGetValue(cubePosition, out var cubeData);
        Destroy(cubeData.worldCube);
        mapCubeData.Remove(cubePosition);

    }

    private void SaveToMapCubeData(CubeData cubeData)
    {
        mapCubeData.Add(cubeData.position, cubeData);
    }

    public void PlaceBlock(Vector3 position)
    {
        Vector3Int mapPosition = position.ToVec3Int();
        if(Physics.CheckBox(mapPosition, Vector3.one * 0.4f, Quaternion.identity) || !IsIngrid(mapPosition)) return;
        CreateBlock(mapPosition, Blocks.Soft);
    }

    private bool IsIngrid(Vector3Int mapPosition)
    {

        return mapPosition.x >= 0 && mapPosition.x < widthMap &&
                mapPosition.y >= 0 && mapPosition.y < heightMap &&
                mapPosition.z >= 0 && mapPosition.z < depthMap;
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

    public float GetBreakTime(Vector3Int cubePositionToDestroy)
    {
        if(mapCubeData.TryGetValue(cubePositionToDestroy, out CubeData cubeData))
        {
            return usedBlockDatabase[cubeData.blockIndex].minigTime;
        }
        return -1;
    }
    private enum Blocks
    {
        //its for human use only -> its indexes of blockData
        //trying make minimal cube data
        Soft,
        Hard,
        Top
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
    public Vector3Int position;//not sure if we need it(is a key)...
    public GameObject worldCube;//this is not optimal
    public ushort blockIndex;//this is block ID from fake block database
    public CubeData(Vector3Int position, GameObject worldCube, ushort blockIndex)
    {
        this.position = position;
        this.worldCube = worldCube;
        this.blockIndex = blockIndex;
    }
}
