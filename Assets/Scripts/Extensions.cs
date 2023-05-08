using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static int ToInt(this float floatNumber)
    {
        return Mathf.RoundToInt(floatNumber);
    } 
    public static Vector3 ToGrid(this Vector3 vector3)
    {
        int x = Mathf.RoundToInt(vector3.x);
        int y = Mathf.RoundToInt(vector3.y);
        int z = Mathf.RoundToInt(vector3.z);
        return new Vector3(x, y, z);
    }
    public static Vector3Int ToVec3Int(this Vector3 vector3)
    {
        int x = Mathf.RoundToInt(vector3.x);
        int y = Mathf.RoundToInt(vector3.y);
        int z = Mathf.RoundToInt(vector3.z);
        return new Vector3Int(x, y, z);
    }
    public static Vector3Int Get1DIndexToVec3(int index)
    {
        //16 width (X)
        //256 height (Y)
        //16 depth (Z)
        int x = index & 15;
        int y = (index >> 4) & 255;
        int z = index >> 12;
        return new Vector3Int(x, y, z);
    }
}
