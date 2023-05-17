using System.Collections.Generic;
using UnityEngine;

//this is chunk data 
//like in minecraft chunk is 16x16x256
// it is 65 536 values(cubes) in 1D indexing and its ushort(16 bits)
//so its only half size of int
namespace Map
{
    public class Chunk
    {
        private Vector2Int position;
        public Vector2Int Position => position;
        public ushort[] cubes;
        public List<ushort> showedNodes;
        public List<uint> changedNodesData;
        public Chunk(Vector2Int position)
        {
            changedNodesData = new();
            showedNodes = new();
            this.position = position;
            cubes = new ushort[65536];                  //number of cubes
        }
    }
}

