using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem3D<TGridValue>
{
    private int width;
    private int height;
    private int depth;
    private TGridValue[,,] gridValues;
    private Vector3Int[] neighbourOffsets = {
        new Vector3Int(-1,0,0),
        new Vector3Int(1,0,0),
        new Vector3Int(0,-1,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,0,-1),
        new Vector3Int(0,0,1)
    };
    public GridSystem3D(int width, int height, int depth, Func<Vector3Int, TGridValue> creatingDelegate)
    {
        gridValues = new TGridValue[width, height, depth];

        this.width = width;
        this.height = height;
        this.depth = depth;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    gridValues[x, y, z] = creatingDelegate.Invoke(new Vector3Int(x, y, z));
                }
            }
        }
    }
    public bool GetGridValue(Vector3Int gridPosition, out TGridValue value)
    {
        value = default;
        if(!IsIngrid(gridPosition)) return false;

        value = gridValues[gridPosition.x, gridPosition.y, gridPosition.z];
        return true;
    }

    private bool IsIngrid(Vector3Int gridPosition)
    {

        return  gridPosition.x >= 0 && gridPosition.x < width &&
                gridPosition.y >= 0 && gridPosition.y < height &&
                gridPosition.z >= 0 && gridPosition.z < depth;
    }
    public bool SetGridValue(Vector3Int gridPosition, TGridValue gridValueNew)
    {
        if(!IsIngrid(gridPosition)) return false;

        gridValues[gridPosition.x, gridPosition.y, gridPosition.z] = gridValueNew;
        return true;
    }
    public IEnumerable<TGridValue> GetNeighbourValues(Vector3Int cubePosition)
    {
        List<TGridValue> returnedList = new();
        foreach (Vector3Int offset in neighbourOffsets)
        {
            Vector3Int checkedPosition = cubePosition + offset;
            if(!IsIngrid(checkedPosition)) continue;
            returnedList.Add(gridValues[checkedPosition.x, checkedPosition.y, checkedPosition.z]);
        }
        return returnedList;
    }    
}
