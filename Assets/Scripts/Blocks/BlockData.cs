using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockData
{
    public int index1D;
    public Vector2Int mapPosition;
    public BlocksSO blockSO;
    public Vector3Int worldPosition;
    public float distance;
    public BlockData(int index, Vector2Int mapPosition, BlocksSO blockSO, Vector3Int worldPosition)
    {
        this.index1D = index;
        this.mapPosition = mapPosition;
        this.blockSO = blockSO;
        this.worldPosition = worldPosition;
        distance = float.MaxValue;
    }
    public void SetDistance(float distance)
    {
        this.distance = distance;
    }
}
