using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this is chunk data unpacker provide the grid system
public class Chunk
{
    //like in minecraft chunk is 16x16x256 it is 65 536 values(cubes) and its ushort
    
    private int width = 16;
    private int height = 256;
    private int depth = 16;
    private Vector2Int position;
    public Vector2Int Position => position;
    public int[] cubes;
    public Chunk(Vector2Int position)
    {
        this.position = position;
        cubes = new int[65536];        
    }
    
}

