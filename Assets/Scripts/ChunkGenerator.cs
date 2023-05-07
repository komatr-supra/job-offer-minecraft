using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator
{
    private int snowIntHeight;
    private WorldConstructor constructor;
    private MapData mapData;
    //test
    private int groundLevel = 10;
    public ChunkGenerator(WorldConstructor constructor)
    {
        this.constructor = constructor;
    }
    public Chunk GenerateChunk(MapData mapData)
    {
        Debug.Log("generating chunk");
        this.mapData = mapData;
        return new Chunk(GetCubeData);
    }
    private CubeData GetCubeData(Vector3Int positionInChunk)
    {
        Vector2Int worldPositionOffset = mapData.position * 16;
        float xPosForPerlin = (positionInChunk.x + worldPositionOffset.x);
        float yPosForPerlin = (positionInChunk.z + worldPositionOffset.y);
        //get terrain height range, how high from wate level is
        int terrainHeightRange = mapData.biome.maximalHeight - mapData.biome.minimalHeight;
        //how high is terrain
        int terraintHeight = (Mathf.PerlinNoise(((xPosForPerlin + mapData.hardOffset.x )* mapData.biome.multiplier), 
                ((yPosForPerlin + mapData.hardOffset.y) * mapData.biome.multiplier)) * terrainHeightRange + groundLevel).ToInt();
        //Debug.Log("ter he" + terraintHeight);
        //how many block is used by soft cubes?
        int softCubesHeight = (Mathf.PerlinNoise(((xPosForPerlin + mapData.softOffset.x) * mapData.biome.multiplier), 
                ((yPosForPerlin + mapData.softOffset.y) * mapData.biome.multiplier)) * 
                mapData.biome.softCubesHeightMax).ToInt();
        softCubesHeight = softCubesHeight > 0 ? softCubesHeight : 1;
        Vector3Int worldPosition = new Vector3Int(positionInChunk.x + worldPositionOffset.x, positionInChunk.y, positionInChunk.z + worldPositionOffset.y);
        //decide type of block, depend on biome and height
        if(positionInChunk.y > terraintHeight) return new CubeData(null, Block.none);
        Block block = GetBlockForGenerate(mapData, positionInChunk, terraintHeight, softCubesHeight);
        return new CubeData(constructor.CreateBlock(worldPosition, block), block);
    }

    private Block GetBlockForGenerate(MapData mapData, Vector3Int positionInChunk, int terraintHeight, int softCubesHeight)
    {
        if(positionInChunk.y > (terraintHeight - softCubesHeight)) return mapData.biome.softBlock;
        else return mapData.biome.hardBlock;
    }

    //todo world builder
    

    
    //todo drop system
    public void BreakBlock(Vector3Int cubePosition)
    {
        

    }

    private void SaveToMapCubeData(CubeData cubeData)
    {
        
    }

    public void PlaceBlock(Vector3 position)
    {
        
    }

    

    
    

    public float GetBreakTime(Vector3Int cubePositionToDestroy)
    {        
        return 1;
    }

    internal void Despawn(List<Chunk> despawnChunks)
    {
        throw new NotImplementedException();
    }

    internal void Spawn(List<Chunk> spawnChunks)
    {
        throw new NotImplementedException();
    }

    
}

public struct CubeData
{
    public GameObject worldCube;//this is not optimal
    public Block block;//this is block ID from fake block database
    public CubeData(GameObject worldCube, Block block)
    {        
        this.worldCube = worldCube;
        this.block = block;
    }
}
