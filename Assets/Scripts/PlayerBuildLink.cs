using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildLink
{
    private WorldConstructor worldConstructor;
    public PlayerBuildLink(WorldConstructor worldConstructor)
    {
        this.worldConstructor = worldConstructor;
    }

    public bool PlaceBlock(Vector3 position, Vector3Int hitBlockWorldPosition, Block block)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        
        stopwatch.Start();        
        worldConstructor.TryPlaceBlock(position, hitBlockWorldPosition, block);
        stopwatch.Stop();
        var elapsed = stopwatch.Elapsed;
        Debug.Log("Plaing block take: " + elapsed.Milliseconds + " miliseconds");

        return false;
    }

    internal void Dig(Vector3Int hitBlockWorldPosition)
    {
        worldConstructor.DestroyBlock(hitBlockWorldPosition);
    }
}
