using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator
{
    private int snowIntHeight;
    private IEnumerable<MapData> mapData;
    //test
    private int groundLevel = 30;
    public IEnumerable<Chunk> GenerateChunk(IEnumerable<MapData> mapDataForCreation)
    {
        foreach (MapData mapData in mapDataForCreation)
        {
            yield return ChunkAssembly(mapData);
        }
        //Debug.Log("generating chunk");
        //this.mapData = mapDataForCreation;
        
        //return new Chunk(GetCubeData);
    }

    private Chunk ChunkAssembly(MapData mapData)
    {
            //chunk is init with 0 = no block
            var chunk = new Chunk(mapData.position);
        //use columns, start type to array at start of terrain
        //position offset in real world
        Vector2Int worldPositionOffset = mapData.position;// * 16; each part solo
        int worldOffsetX = (worldPositionOffset.x << 4);
        int worldOffsetY = (worldPositionOffset.y << 4);
        //go through x, z(in real world) its x, y (in perlin) 16x16
        //get terrain height range, how high from wate level is
        int terrainHeightRange = mapData.biome.maximalHeight - mapData.biome.minimalHeight;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    float xPosForPerlin = (x | worldOffsetX);
                    float yPosForPerlin = (z | worldOffsetY);

                    //how high is terrain
                    int terraintHeight = (Mathf.PerlinNoise(((xPosForPerlin + mapData.hardOffset.x )* mapData.biome.multiplier), 
                    ((yPosForPerlin + mapData.hardOffset.y) * mapData.biome.multiplier)) * terrainHeightRange + groundLevel).ToInt();
                    
                    //how many block of terrain is used by soft cubes?
                    int softCubesHeight = (Mathf.PerlinNoise(((xPosForPerlin + mapData.softOffset.x) * mapData.biome.multiplier), 
                    ((yPosForPerlin + mapData.softOffset.y) * mapData.biome.multiplier)) * mapData.biome.softCubesHeightMax).ToInt();

                    //all positions of terrain
                    int softCubesHeightStart = terraintHeight - softCubesHeight;
                    for (int yPosition = 0; yPosition < terraintHeight; yPosition++)
                    {
                        int index1D = x | (yPosition << 4) | (z << 12);
                        chunk.cubes[index1D] = yPosition > softCubesHeightStart ? 1 : 2;//type of cube
                    }

                }
            }
            //look for saves
            if(FakeSaveSystem.Instance.TryGetSave(chunk.Position, out uint[] saves))
            {
                foreach (var save in saves)
                {
                    uint index = save >> 16;
                    uint blockID = save & 65535;

                    chunk.cubes[(int)index] = (int)blockID;
                }
            }
            return chunk;
        }
    
}

