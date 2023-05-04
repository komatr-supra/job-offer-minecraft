using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static int ToInt(this float floatNumber)
    {
        return Mathf.RoundToInt(floatNumber);
    } 
}
