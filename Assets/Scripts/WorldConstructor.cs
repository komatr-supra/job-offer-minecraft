using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldConstructor
{
    private Queue<GameObject> unusedCubes;
    private int StartCapacity = 100000;
    private Dictionary<Vector3Int, GameObject> usedCubes;
    private int[][] neighbours;
    public WorldConstructor()
    {
        usedCubes = new Dictionary<Vector3Int, GameObject>();
        unusedCubes = new Queue<GameObject>();

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < StartCapacity; i++)
        {
            var v = GameObject.CreatePrimitive(PrimitiveType.Cube);
            v.SetActive(false);
            unusedCubes.Enqueue(v);
        }
        stopwatch.Stop();
        var elapsed = stopwatch.Elapsed;
        Debug.Log("Prepare cubes pool of " + StartCapacity + "cubes in: " + elapsed.Milliseconds + " miliseconds");

        stopwatch.Restart();
        PrepareNeighbours();
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Debug.Log("Prepare neighbour lookup array created time: " + elapsed.Milliseconds + " miliseconds");
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
        neighbours = new int[65536][];
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
            neighbours[index] = neighbourList.ToArray();
        }
    }

    public void SpawnChunk(Chunk chunk)
    {
        for (int index = 0; index < chunk.cubes.Length; index++)
        {
            //check neighbours, if any of them is 0(empty, then this must be visible)
            //skip empty nodes
            if(chunk.cubes[index] == 0) continue;
            foreach (int neighbourIndex in neighbours[index])
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
    public void CreateBlock(Vector3Int worldPosition, Block block)
    {
        BlocksSO blockData = FakeDatabase.Instance.GetBlock(block);
        if(unusedCubes.TryDequeue(out var cube))
        {
            cube.transform.position = worldPosition;
            cube.layer = 3;
            cube.GetComponent<MeshRenderer>().material = blockData.material;
            cube.SetActive(true);
        }
        else
        {
            Debug.Log("no more cubes in pool");
        }
    }
}
