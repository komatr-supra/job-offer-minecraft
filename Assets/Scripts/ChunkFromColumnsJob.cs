using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


public struct ChunkFromColumnsJob : IJob
{
    [ReadOnly]
    public NativeArray<uint> columns;
    [WriteOnly]
    public NativeArray<ushort> cubes;

    public void Execute()
    {
        for (int columnIndex = 0; columnIndex < 256; columnIndex++)
        {
            //position of column
            int x = columnIndex & 15;
            int z = columnIndex >> 4;

            //create column
            int hardStop = (int)(columns[columnIndex] & 65535);
            int hardHeigh = hardStop & 255;
            ushort hardBlockType = (ushort)(hardStop >> 8);
            int softStop = (int)(columns[columnIndex] >> 16);
            int softHeight = softStop & 255;
            ushort softBlockType = (ushort)(softStop >> 8);
            //create all
            for (int y = 0; y < softStop; y++)
            {
                int index1D = x | y << 4 | z << 12;
                cubes[index1D] = y < hardHeigh ? hardBlockType : softBlockType; 
            }
        }
        
    }
}
