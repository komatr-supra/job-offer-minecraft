using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager
{
    [SerializeField] private GameObject cube;
    private int seed;
    
    private ChunkGenerator chunkGenerator;
    private MapGenerator mapGenerator;
    private WorldConstructor worldConstructor;
    private Vector2Int activeChunkMapPos;//this is position of the middle active chunk on the map, grid 3x3 is active
    public Vector2Int ActiveChunkMapPos => activeChunkMapPos;
    private Vector2Int[] activeChunksOffsets = {
        new Vector2Int(0,0),//this
        new Vector2Int(-1, 0),//left
        new Vector2Int(-1, -1),//left top
        new Vector2Int(0, -1),//top
        new Vector2Int(1, -1),//right top
        new Vector2Int(1, 0),//right
        new Vector2Int(1, 1),//right bot
        new Vector2Int(0, 1),//bot
        new Vector2Int(-1, 1),//left bot
    };
    private List<Chunk> activeChunks;
    public MapManager(int seed, Action<Vector2Int> onCharacterPositionChange)
    {
        onCharacterPositionChange += RecalculateActiveChunk;
        //seed handle
        if(seed == -1) seed = (int)(System.DateTime.Now.Ticks);
        UnityEngine.Random.InitState(seed);
        Debug.Log(seed);
        this.seed = seed;

        //prepare 
        mapGenerator = new MapGenerator();
        worldConstructor = new WorldConstructor();
        chunkGenerator = new ChunkGenerator();

    }
    public void Generate(Vector2Int pos)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        
        stopwatch.Start();        
        var v = mapGenerator.GetMapData(pos);
        stopwatch.Stop();
        TimeSpan elapsed = stopwatch.Elapsed;
        Debug.Log("Map data created time: " + elapsed.Milliseconds + " miliseconds");

        stopwatch.Restart();
        var chunk = chunkGenerator.GenerateChunk(v);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Debug.Log("Chunk data created time: " + elapsed.Milliseconds + " miliseconds");

        stopwatch.Restart();
        worldConstructor.SpawnChunk(chunk);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Debug.Log("World buildier time to create chunk: " + elapsed.Milliseconds + " miliseconds");
        //RecalculateActiveChunk(Vector2Int.zero);
    }
    
    private void RecalculateActiveChunk(Vector2Int newChunkPosition)
    {
        Debug.Log("chunks change");
        /*
        activeChunkMapPos = newChunkPosition;
        //not optimal? should be adjustable
        Vector2Int[] positionsToPrepare = new Vector2Int[9];//nine squeres
        for (int i = 0; i < 9; i++)
        {
            positionsToPrepare[i] = activeChunkMapPos + activeChunksOffsets[i];
        }
        //look at all activeChunk list
        List<Chunk> despawnChunks = new();
        foreach (var currentChunk in activeChunks)
        {
            //dont need this chunk
            if(!positionsToPrepare.Contains(currentChunk.Position))
            {
                despawnChunks.Add(currentChunk);
            }
        }
        foreach (var item in despawnChunks)
        {
            activeChunks.Remove(item);
        }
        chunkGenerator.Despawn(despawnChunks);
        //spawn new chunks
        List<Chunk> spawnChunks = new();
        foreach (var currentChunk in activeChunks)
        {
            if(!positionsToPrepare.Contains(currentChunk.Position)) spawnChunks.Add(currentChunk);
        }
        chunkGenerator.Spawn(spawnChunks);
        Debug.Log("new chunk should be callcualated, despawn old one and use his block to build next");
        */
    }
    
}
