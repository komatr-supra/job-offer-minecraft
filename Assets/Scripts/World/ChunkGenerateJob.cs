using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Map
{
    public struct ChunkGenerateJob : IJobParallelFor
    {
        [ReadOnly]
        public float2 hardOffset;
        [ReadOnly]
        public float2 softOffset;
        [ReadOnly]
        public int terrainHeightRange;
        [ReadOnly]
        public int groundLevel;
        [ReadOnly]
        public float biomeMultiplier;
        [ReadOnly]
        public int2 worldOffset;
        [ReadOnly]
        public int biomeSoftCubesHeightMax;
        [ReadOnly]
        public int snowHeight;
        [WriteOnly]
        public NativeArray<uint> columnData;
        public void Execute(int index)      //16 x 16    X  Z
        {
            float2 float2one = new float2(1,1);
            int x = index & 15;
            int z = index >> 4;
            float2 realPositon = new float2(x | worldOffset.x, z | worldOffset.y);
            //how high is terrain
            float terraintHeight = (noise.pnoise(((realPositon + hardOffset)* biomeMultiplier), 
                    float2one) * terrainHeightRange + groundLevel);
            
            //how many block of terrain is used by soft cubes?
            float softCubesHeight = (noise.pnoise(((realPositon + softOffset) * biomeMultiplier), 
                    float2one) * biomeSoftCubesHeightMax);

            //all positions of terrain
            float softCubesHeightStart = terraintHeight - softCubesHeight;

            uint lastCube = 2;
            int saveNO = 0;
            uint data = 0;
            for (uint y = 0; y < 256; y++)            
            {
                uint newCube = (y < softCubesHeightStart ? 2u : y < snowHeight ? 1u : 3u);
                //cubes changed -> write max few changes stone, dirt, air
                if(newCube != lastCube)
                {
                    lastCube = newCube;
                    data = y << (16 * saveNO) | newCube << ((16 * saveNO) + 8);
                    saveNO++;
                }
            }
            columnData[index] = data;
            
        }
    }
}
