using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Timers;
using Unity.Collections;

namespace Map
{
    public class WorldConstructor
    {
        
        private BlockPool blockPool;
        MapDataProvider mapDataProvider;
        public WorldConstructor(MapDataProvider mapDataProvider)
        {
            this.mapDataProvider = mapDataProvider;
            blockPool = new();
        }
        public void SpawnChunk(Chunk chunk)
        {
            Debug.Log("spawning chunk");
            //get all chunks
            Chunk[] chunks = new Chunk[5];
            chunks[0] = chunk;
            Vector2Int chunkPos = chunk.Position;
            Vector2Int leftChunkPos = new Vector2Int(-1, 0) + chunkPos;
            mapDataProvider.GetChunk(leftChunkPos, out Chunk chunk1);
            chunks[1] = chunk1;//left
            Vector2Int rightChunkPos = new Vector2Int(1, 0) + chunkPos;
            mapDataProvider.GetChunk(rightChunkPos, out Chunk chunk2);
            chunks[2] = chunk2; //right
            Vector2Int frontChunkPos = new Vector2Int(0, -1) + chunkPos;
            mapDataProvider.GetChunk(frontChunkPos, out Chunk chunk3);
            chunks[3] = chunk3; //front
            Vector2Int backChunkPos = new Vector2Int(0, 1) + chunkPos;
            mapDataProvider.GetChunk(backChunkPos, out Chunk chunk4);
            chunks[4] = chunk4; //back
            //chunks[5] = chunk; //up down => empty

            for (int index = 0; index < chunks[0].cubes.Length; index++)
            {
                //check neighbours, if any of them is 0(empty, then this must be visible)
                //skip empty nodes
                
                if(chunks[0].cubes[index] == 0) continue;
                
                int[] neighbourDatas = mapDataProvider.GetNeighboursData(index).ToArray();
                foreach (int neighbourData in neighbourDatas)
                {
                    //Debug.Log("neighbour");
                    int chunkIndex = (neighbourData >> 16) & 5;
                    if(chunkIndex == 5) continue;
                    if(chunkIndex < 0) {Debug.Log("mimo" + chunkIndex); }
                    int neighbourIndex = (neighbourData & 65535);
                    if(chunkIndex > 5) Debug.Log("hovno" + neighbourIndex + " " + neighbourData);
                    var v = chunks[chunkIndex];
                    int blockData = v.cubes[neighbourIndex];
                    if (blockData == 0)
                    {
                        CreateBlock(chunks[0].Position, index, (Block)blockData);
                        break;
                    }
                }
                
            }
            
        }

        private void CreateBlock(Vector2Int position, int index, Block block)
        {
            Vector3Int worldPos = new Vector3Int(position.x >> 4, 0, position.y >> 4) + mapDataProvider.GetPositionInChunk(index);
            if(worldPos.x < 0 || worldPos.z < 0) Debug.Log("creating block" + worldPos);
            CreateBlock(worldPos, block);
        }

        

        /*
public void DespawnChunk(Chunk chunk)
{
   if(chunk == null) return;
   foreach (int blockIndex in chunk.showedNodes)
   {
       DestroyBlock(chunk, blockIndex);
   }
}*/
        public void CreateBlock(Vector3Int worldPosition, Block block)
        {
            blockPool.SetCube(worldPosition, block);
        }   
        public bool DestroyBlock(Vector3Int worldPosition)
        {   
            return blockPool.DisableCube(worldPosition);            
        }

        internal void DespawnChunk(Chunk chunk)
        {
            throw new NotImplementedException();
        }
    }
    
}
