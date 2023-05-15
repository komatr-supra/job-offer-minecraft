using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Map;
public class BlockPool
{
    private const int prewarmCubesAmount = 150000;
    private Queue<GameObject> unusedCubes;
    private Dictionary<Vector3Int, GameObject> usedCubes;
    private List<Vector3Int> positionsToDestroy;
    private const int MAX_DELETED_COUNT = 100;
    private Coroutine destroyCubeCoroutine;
    private const int MAX_CREATED_CHUNKS = 3;
    private Dictionary<Vector2Int, uint[]> chunksToCreate;
    private Coroutine createCubeCoroutine;
    public BlockPool()
    {
        usedCubes = new Dictionary<Vector3Int, GameObject>();
        unusedCubes = new Queue<GameObject>();
        positionsToDestroy = new();
        chunksToCreate = new();
        for (int i = 0; i < prewarmCubesAmount; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.layer = 3;
            cube.SetActive(false);
            cube.transform.SetParent(FakeDatabase.Instance.transform);
            unusedCubes.Enqueue(cube);
        }
    }
    public bool SetCube(Vector3Int worldPosition, Block block)
    {
        if(usedCubes.ContainsKey(worldPosition)) return false;
        var cube = unusedCubes.Dequeue();
        cube.transform.position = worldPosition;
        BlocksSO blockData = FakeDatabase.Instance.GetBlock(block);
        cube.GetComponent<MeshRenderer>().material = blockData.material;
        cube.SetActive(true);
        usedCubes.Add(worldPosition, cube);
        return true;
    }
    public bool SetCube(Vector3Int worldPosition, BlocksSO blockSO)
    {
        if(usedCubes.ContainsKey(worldPosition)) return false;
        var cube = unusedCubes.Dequeue();
        cube.transform.position = worldPosition;
        cube.GetComponent<MeshRenderer>().material = blockSO.material;
        cube.SetActive(true);
        usedCubes.Add(worldPosition, cube);
        return true;
    }
    public bool DisableCube(Vector3Int worldPosition)
    {
        if(!usedCubes.ContainsKey(worldPosition))
        {
            //Debug.Log("cant destroy block at " + worldPosition );
            return false;
        }
        var cube = usedCubes[worldPosition];
        cube.SetActive(false);
        usedCubes.Remove(worldPosition);
        unusedCubes.Enqueue(cube);
        return true;
    }
    public void DisableCubeAsync(List<Vector3Int> positions)
    {
        positionsToDestroy.AddRange(positions);
        if(destroyCubeCoroutine == null)
        {
            destroyCubeCoroutine = WorldTimer.Instance.StartCoroutine(DestroyingCoroutine());
        }
    }
    private IEnumerator DestroyingCoroutine()
    {
        while (positionsToDestroy.Count > 0)
        {
            //get required lengh
            int repeat = Mathf.Min(MAX_DELETED_COUNT, positionsToDestroy.Count);
            for (int i = 0; i < repeat; i++)
            {
                DisableCube(positionsToDestroy[0]);
                positionsToDestroy.RemoveAt(0);
            }
            yield return null;            
        }
        destroyCubeCoroutine = null;
    }

    internal void SetCubeAsync(uint[] filledCubes, Vector2Int chunkPos)
    {
        //Debug.Log(chunkPos);
        //add to dictionary
        chunksToCreate.TryAdd(chunkPos, filledCubes);
        if(createCubeCoroutine == null)
        {
            createCubeCoroutine = WorldTimer.Instance.StartCoroutine(CreatingCoroutine());
        }
        
    }
    private IEnumerator CreatingCoroutine()
    {
        while (chunksToCreate.Count > 0)
        {
            int repeat = Mathf.Min(MAX_CREATED_CHUNKS, chunksToCreate.Count);
            for (int i = 0; i < repeat; i++)
            {
                var chunkData = chunksToCreate.First();
                Vector2Int chunkOffset = chunkData.Key;
                var blocksIndex = chunkData.Value;
                foreach (var nodeData in blocksIndex)
                {
                    uint blockIndex = nodeData >> 16;
                    uint blockID = nodeData & 65535;
                    Vector3Int blockPos = MapDataProvider.GetPositionInChunk((int)blockIndex);
                    Vector3Int offset = new Vector3Int(chunkOffset.x << 4, 0, chunkOffset.y << 4);
                    SetCube(offset + blockPos, (Block)blockID);
                
                //Debug.Log("creating at " + offset + blockPos);
                }
                chunksToCreate.Remove(chunkOffset);
                //Debug.Log("new chunk created");
            }
            
            yield return null;
        }
        createCubeCoroutine = null;
    }
}
