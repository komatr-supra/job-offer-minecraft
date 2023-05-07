using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    //this is basicly world save data
    //solid blocks
    private Vector2 solidOffset;
    //soft blocks
    private Vector2 softOffset;
    public MapGenerator()
    {        
        solidOffset.x = UnityEngine.Random.Range(0f, 1000000f);
        solidOffset.y = UnityEngine.Random.Range(0f, 1000000f);
        softOffset.x = UnityEngine.Random.Range(0f, 1000000f);
        softOffset.y = UnityEngine.Random.Range(0f, 1000000f);
        Debug.Log(softOffset);
        Debug.Log(solidOffset);
    }
    public MapData GetMapData(Vector2Int position)
    {
        BiomesSO biome = FakeDatabase.Instance.GetBiome((Biome)1);//test
        return new MapData(biome, position, softOffset, solidOffset);
    }
}
