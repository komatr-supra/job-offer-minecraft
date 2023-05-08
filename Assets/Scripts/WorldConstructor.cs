using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldConstructor
{
    private int[][] neighboursLookup;
    private List<Chunk> chunks;
    private BlockPool blockPool;
    public WorldConstructor()
    {        
        chunks = new();
        blockPool = new();
        PrepareNeighbours();
    }

    private void PrepareNeighbours()
    {
        Vector3Int[] offsets = {
            new Vector3Int(-1,0,0),
            new Vector3Int(1,0,0),
            new Vector3Int(0,-1,0),
            new Vector3Int(0,1,0),
            new Vector3Int(0,0,-1),
            new Vector3Int(0,0,1)
        };
        neighboursLookup = new int[65536][];
        for (int index = 0; index < 65536; index++)
        {            
            Vector3Int currentPos = GetPositionInChunk(index);

            List<int> neighbourList = new();
            foreach (Vector3Int offset in offsets)
            {
                //if is in grid, add
                Vector3Int neighbourPos = offset + currentPos;
                if(neighbourPos.x < 0 || neighbourPos.x > 15 || 
                    neighbourPos.y < 0 || neighbourPos.y > 255 ||
                    neighbourPos.z < 0 || neighbourPos.z > 15
                    ) continue;
                //get 1d pos
                int neighbourIndex = neighbourPos.x + (neighbourPos.y << 4) + (neighbourPos.z << 12);
                neighbourList.Add(neighbourIndex);
            }
            neighboursLookup[index] = neighbourList.ToArray();
        }
    }

    public void SpawnChunk(Chunk chunk)
    {
        chunks.Add(chunk);
        for (int index = 0; index < chunk.cubes.Length; index++)
        {
            //check neighbours, if any of them is 0(empty, then this must be visible)
            //skip empty nodes
            if(chunk.cubes[index] == 0) continue;
            foreach (int neighbourIndex in neighboursLookup[index])
            {
                if (chunk.cubes[neighbourIndex] == 0)
                {
                    Vector3Int cubeWorldPosition = GetPositionInChunk(index) + new Vector3Int(chunk.Position.x* 16, 0, chunk.Position.y * 16);
                    CreateBlock(cubeWorldPosition, (Block)chunk.cubes[index]);
                    break;
                }
            }

        }
    }
    public void DespawnChunk(Chunk chunk)
    {

    }
    
    private Vector3Int GetPositionInChunk(int index)
    {
        //16 width (X)
        //256 height (Y)
        //16 depth (Z)
        int x = index & 15;
        int y = (index >> 4) & 255;
        int z = index >> 12;
        return new Vector3Int(x, y, z);
    }
    //force creating (from complete data)
    private void CreateBlock(Vector3Int worldPosition, Block block)
    {
        blockPool.SetCube(worldPosition, block);
    }
    //runtime creating (set data and check block around)
    private void PlaceBlock(Vector3Int worldPosition, Block block)
    {
        CreateBlock(worldPosition, block);
        SetBlockAtWorldPosition(worldPosition, block);        
    }
    private void SetBlockAtWorldPosition(Vector3Int worldPosition, Block block)
    {
        Chunk chunk = GetChunk(worldPosition);
        int index = GetIndex(worldPosition);
        chunk.cubes[index] = (int)block;
        //check neighbours, if they need update
        UpdateCubesAroundIndexInChunk(chunk, index);
    }
    //method for player, or any raycast (this will place block in perfect grid and check if it is possible)
    public bool TryPlaceBlock(Vector3 positionOnCube, Vector3Int targetedWorldCubePos, Block block)
    {
        //get chunk        
        Chunk chunk = GetChunk(targetedWorldCubePos);
        Vector3Int chunkOffset = GetChunkOffset(chunk.Position);

        //get chunk index
        Vector3Int cubePosInChunk = targetedWorldCubePos;
        int index = GetIndex(cubePosInChunk);

        //get all neighbours
        List<int> freeNeighbours = new();
        foreach (int neighbourID in neighboursLookup[index])
        {
            if (chunk.cubes[neighbourID] == 0) freeNeighbours.Add(neighbourID);
        }
        // no neighbours avaliable
        if (freeNeighbours.Count == 0) return false;
        
        //get nearest free cube
        int chunkIndex = freeNeighbours.OrderBy(x => Vector3.Distance(positionOnCube, GetVec3(x) + chunkOffset)).First();
        Vector3Int positionForNewBlock = GetWorldPosition(chunk, chunkIndex);
        if (Physics.CheckBox(positionForNewBlock, Vector3.one * 0.49f, Quaternion.identity)) return false;
        PlaceBlock(positionForNewBlock, block);

        return true;
    }
    private Vector3Int GetWorldPosition(Chunk chunk, int chunkIndex)
    {
        return GetVec3(chunkIndex) + GetChunkOffset(chunk.Position);
    }

    private Vector3Int GetVec3(int index)
    {
        //16 width (X)
        //256 height (Y)
        //16 depth (Z)
        int x = index & 15;
        int y = (index >> 4) & 255;
        int z = index >> 12;
        return new Vector3Int(x, y, z);
    }

    private static int GetIndex(Vector3Int worldPosition)
    {
        return (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
    }    
    private static Vector3Int GetChunkOffset(Vector2Int mapPosition)
    {
        return new Vector3Int(mapPosition.x << 4, 0, mapPosition.y << 4);
    }
    private static Vector2Int GetMapPosition(Vector3Int targetedWorldCubePos)
    {
        return new Vector2Int(targetedWorldCubePos.x >> 4, targetedWorldCubePos.z >> 4);
    }

    private void UpdateCubesAroundIndexInChunk(Chunk chunk, int index)
    {        
        //get near nodes with block
        var neighbours = neighboursLookup[index];
        //check used neighbour and recalculate visibility
        foreach (int neighbourIndex in neighbours.Where(x => chunk.cubes[x] != 0))
        {
            RefreshCubeVisibility(chunk, neighbourIndex);
        }
    }

    private void RefreshCubeVisibility(Chunk chunk, int index)
    {
        bool isInvisible = true;
        foreach (int neighbour in neighboursLookup[index])
        {
            //neighbour cube is used -> keep looking
            if(chunk.cubes[neighbour] != 0) continue;
            //any of the node is free
            isInvisible = false;
            break;
        }
        Vector3Int worldPosition = GetWorldPosition(chunk, index);
        if(isInvisible)
        {
            //hide cube
            Debug.Log("hiding cube " + worldPosition);
            blockPool.DisableCube(worldPosition);
        }
        else
        {
            //show cube
            blockPool.SetCube(worldPosition, (Block)chunk.cubes[index]);
        }
    }

    private Chunk GetChunk(Vector3Int worldPosition)
    {
        foreach (var chunk in chunks)
        {
            if(chunk.Position == GetMapPosition(worldPosition)) return chunk;
        }
        Debug.LogError("want choose not loaded chunk!");
        return default;
    }

    internal void DestroyBlock(Vector3Int hitBlockWorldPosition)
    {        
        //get chunk
        Chunk chunk = GetChunk(hitBlockWorldPosition);
        blockPool.DisableCube(hitBlockWorldPosition);
        int index = GetIndex(hitBlockWorldPosition);
        chunk.cubes[index] = 0;
        UpdateCubesAroundIndexInChunk(chunk, index);
    }
}
