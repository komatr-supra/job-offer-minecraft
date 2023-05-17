using Unity.Collections;
using Unity.Jobs;

//this is for job system, calculate vision of the cubes
//cubes with all neighbours are not visible
namespace Map
{    
    public struct ChunkCreateJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<int> neighboursLookupDataArray;
        [ReadOnly]
        public NativeArray<ushort> blockDataMain;
        [ReadOnly]
        public NativeArray<ushort> blockDataLeft;
        [ReadOnly]
        public NativeArray<ushort> blockDataRight;
        [ReadOnly]
        public NativeArray<ushort> blockDataFront;
        [ReadOnly]
        public NativeArray<ushort> blockDataBack;
        [WriteOnly]
        public NativeArray<uint> usedBlocks;
        public void Execute(int index)
        {
            //check neighbours, if any of them is 0(empty, then this must be visible)
            //skip empty nodes        
            if(blockDataMain[index] == 0) return;
            
            int[] neighbourDatas = GetNeighboursData(index);
            foreach (int neighbourData in neighbourDatas)
            {
                int neighbourIndex = (neighbourData & 65535);
                int chunkIndex = (neighbourData >> 16);
                chunkIndex = chunkIndex & 7;
                if(chunkIndex == 5) continue;
                var v = GetBlockData(chunkIndex);
                int blockData = v[neighbourIndex];
                //block data is int with cube info(fake database index)
                // 0 is empty neighbour -> show actual index
                if (blockData == 0)
                {
                    //add THIS INDEX(index) as used cube -> this block is same as used block
                    //its a bit odd, but i want index and block ID from fakedatabase
                    
                    usedBlocks[index] = ((uint)index) << 16 | (uint)blockDataMain[index];
                    break;
                }
            }
        }
        private int[] GetNeighboursData(int index)
        {
            int[] neighbourArray = new int[6];
            int x = index & 65535;//% 65536;
            for (int i = 0; i < 6; i++)
            {
                neighbourArray[i] = neighboursLookupDataArray[(i << 16) | x];            
            }
            return neighbourArray;
        }
        //this must be same as it is in chunk generator
        private NativeArray<ushort> GetBlockData(int i)
        {
            switch (i)
            {
                case 1 : return blockDataLeft; 
                case 2 : return blockDataRight;
                case 3 : return blockDataFront;
                case 4 : return blockDataBack;

                default: return blockDataMain; 
            }
        }
    }
}
