using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class MapDataProvider : IDisposable
{
    public NativeArray<int> neighboursLookupDataArray;
    private Dictionary<Vector2Int, Chunk> activeChunks;
    private ChunkGenerator chunkGenerator;
    private MapGenerator mapGenerator;
    private int mapDrawRadius;
    public int VisionRange => mapDrawRadius;                // this is +- value (its double size square) ans should be in setup
    private Vector3Int[] offsets = {
        new Vector3Int(-1,0,0),//left
        new Vector3Int(1,0,0),//right
        new Vector3Int(0,-1,0),//down
        new Vector3Int(0,1,0),//up
        new Vector3Int(0,0,-1),//front
        new Vector3Int(0,0,1)//back
    };
    public MapDataProvider(int mapDrawRadius)
    {
        this.mapDrawRadius = mapDrawRadius;
        activeChunks = new();
        chunkGenerator = new ChunkGenerator();
        mapGenerator = new MapGenerator();

        PrepareNeighbours();
    }
    private void PrepareNeighbours()
    {
        neighboursLookupDataArray = new NativeArray<int>(65536 * 6, Allocator.Persistent);
        for (int x = 0; x < 65536; x++)
        {            
            Vector3Int currentPos = GetPositionInChunk(x);
            //1D index
            //x = 65536 - block count
            //y = 6 - neighbours for each(offsets)
            //index = x + y * 65536
            //
            for (int y = 0; y < offsets.Length; y++)
            {
                //this is neighbour
                Vector3Int neighbourPos = offsets[y] + currentPos;                    
                ushort chunkIndex = 0;//current chunk
                //skip Y neighbour out of array(chunks is not connected vertically)
                if(neighbourPos.y < 0 || neighbourPos.y > 255)
                {
                    chunkIndex = 5; //no neighbour, but for job system is needed consistent indexing
                    neighbourPos.y = 0;
                }
                //check x                    
                else if(neighbourPos.x < 0)
                {
                    //left chunk
                    chunkIndex = 1;
                    neighbourPos.x = 15;
                }
                else if(neighbourPos.x > 15)
                {
                    //right chunk
                    chunkIndex = 2;
                    neighbourPos.x = 0;
                }
                else if(neighbourPos.z < 0)
                {
                    //front chunk
                    chunkIndex = 3;
                    neighbourPos.z = 15;
                }
                else if(neighbourPos.z > 15)
                {   
                    //back chunk
                    chunkIndex = 4;
                    neighbourPos.z = 0;
                }
                int neighbour1DIndexInHisChunk = neighbourPos.x | (neighbourPos.y << 4) | (neighbourPos.z << 12);
                
                int index1D = x + (y * 65536);
                //merge datas
                if(chunkIndex > 5 || chunkIndex < 0) Debug.Log("v pici index ");
                neighboursLookupDataArray[index1D] = ((chunkIndex << 16) | neighbour1DIndexInHisChunk);
            }
        }
    }
    public IEnumerable<int> GetNeighboursData(int index)
    {
        //index = index & 131071;
        //go throught lookupArray
        int x = index & 65535;//% 65536;
        for (int i = 0; i < 6; i++)
        {
            var v = neighboursLookupDataArray[(i << 16) | x];// i * 65536 + x ... x is up to 16
            if(v >> 16 < 0 || v >> 16 > 5)Debug.Log("napicu data v neighbours lookup data array");
            yield return v;
        }
    }
    public Vector3Int GetPositionInChunk(int index)
    {
        //16 width (X)
        //256 height (Y)
        //16 depth (Z)
        int x = index & 15;
        int y = (index >> 4) & 255;
        int z = index >> 12;
        return new Vector3Int(x, y, z);
    }

    public bool GetChunk(Vector2Int mapPosition, out Chunk chunk)
    {        
        if(activeChunks.TryGetValue(mapPosition, out chunk)) return true;
        
        Vector2Int[] map = { mapPosition };
        var mapData = mapGenerator.GetMapDatas(map);
        chunk = chunkGenerator.GenerateChunk(mapData).Single();
        activeChunks.Add(chunk.Position, chunk);
        return false;        
    }

    public void SetCubeDataInChunk(Vector3Int worldPosition, int blockIndexInDatabase)
    {
        

    }

    private static Vector2Int GetMapPosition(Vector3Int worldPosition)
    {
        return new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
    }

    public IEnumerable<Vector2Int> GetUsedMapPositions(Vector2Int playerChunkPosition)
    {        
        for (int xx = -mapDrawRadius; xx <= mapDrawRadius; xx++)
        {
            for (int yy = -mapDrawRadius; yy <= mapDrawRadius; yy++)
            {
                yield return (new Vector2Int(playerChunkPosition.x + xx, playerChunkPosition.y + yy));
            }
        }        
    }

    public bool SetBlockData(Vector3Int worldPosition, Block block)
    {
        Vector2Int mapPosition = GetMapPosition(worldPosition);
        //chunk is not ready
        if(!GetChunk(mapPosition, out Chunk chunk)) return false;
        int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
        chunk.cubes[index] = (int)block;
        return true;
        //update neighbours
    }
    public BlockData GetBlockDatas(Vector3Int worldPosition)
    {
        //get neighbours           
        Vector2Int mapPosition = GetMapPosition(worldPosition); 
        GetChunk(mapPosition, out Chunk chunk);
        int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
        BlocksSO blockType = GetBlockSO(chunk, index);
        return new BlockData(index, mapPosition, blockType, worldPosition);
    }
    public IEnumerable<BlockData> GetNeighbourDatas(Vector3Int worldPosition)
    {
        //find neighbour information
        foreach (Vector3Int offset in offsets)
        {
            Vector3Int targetPosition = worldPosition + offset;
            yield return GetBlockDatas(targetPosition);
        }
    }
    private BlocksSO GetBlockSO(Chunk chunk, int index1D)
    {
        return FakeDatabase.Instance.GetBlock((Block)chunk.cubes[index1D]);
    }
    internal void UpdateNeighbours(Chunk chunk, int index)
        {
            /*
            foreach (int neighbourIndex in neighboursLookupArray[index])
            {
                //empty nodes have got no visual (database[0] is empty)
                int neighbourBlockDatabaseID = chunk.cubes[neighbourIndex];
                Debug.Log("this block have data: " + neighbourBlockDatabaseID);
                if (neighbourBlockDatabaseID == 0) continue;
                UpdateVisual(chunk, neighbourIndex);
            }
            */
        }

        private void UpdateVisual(Chunk chunk, int index)
        {
            /*
            foreach (int neighbourIndex in neighboursLookupArray[index])
            {
                //this is neighbours of neighbours
                //one of the neighbours is empty and this cube must be visible
                if (chunk.cubes[neighbourIndex] == 0)
                {
                    int idBlock = chunk.cubes[index];
                    Vector3Int neighbourWorldPosition = GetPositionInChunk(index) + new Vector3Int(chunk.Position.x << 4, 0, chunk.Position.y << 4);
                    blockPool.SetCube(neighbourWorldPosition, (Block)idBlock);
                    Debug.Log("Setting block " + neighbourWorldPosition + " to " + idBlock + " update");
                    //this will break this neighbour(neighbour of main block)
                    return;
                }

            }
            */
        }

    internal void RemoveChunk(Chunk chunk)
    {
        activeChunks.Remove(chunk.Position);
    }

    internal void AddChunk(Chunk chunk)
    {
        activeChunks.TryAdd(chunk.Position, chunk);
        Debug.LogWarning("new chunk " + chunk.Position);
    }

    public void Dispose()
    {
        neighboursLookupDataArray.Dispose();
    }
}
