using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Timers;

namespace Map
{
    public class WorldConstructor
    {
        private int[][] neighboursLookup;
        private BlockPool blockPool;
        public WorldConstructor()
        {   
            blockPool = new();
            PrepareNeighbours();
            
        }

        private void PrepareNeighbours()
        {
            Vector3Int[] offsets = {
                new Vector3Int(-1,0,0),
                new Vector3Int(1,0,0),
                new Vector3Int(0,-1,0),
                new Vector3Int(0,1,0),
                new Vector3Int(0,0,-1),
                new Vector3Int(0,0,1)
            };
            neighboursLookup = new int[65536][];
            for (int index = 0; index < 65536; index++)
            {            
                Vector3Int currentPos = GetPositionInChunk(index);

                List<int> neighbourList = new();
                foreach (Vector3Int offset in offsets)
                {
                    //if is in grid, add
                    Vector3Int neighbourPos = offset + currentPos;
                    if(neighbourPos.x < 0 || neighbourPos.x > 15 || 
                        neighbourPos.y < 0 || neighbourPos.y > 255 ||
                        neighbourPos.z < 0 || neighbourPos.z > 15
                        ) continue;
                    //get 1d pos
                    int neighbourIndex = neighbourPos.x + (neighbourPos.y << 4) + (neighbourPos.z << 12);
                    neighbourList.Add(neighbourIndex);
                }
                neighboursLookup[index] = neighbourList.ToArray();
            }
        }

        public void SpawnChunks(Chunk[] chunks)
        {
            foreach (var chunk in chunks)
            {
                for (int index = 0; index < chunk.cubes.Length; index++)
                {
                    //check neighbours, if any of them is 0(empty, then this must be visible)
                    //skip empty nodes
                    if(chunk.cubes[index] == 0) continue;
                    foreach (int neighbourIndex in neighboursLookup[index])
                    {
                        if (chunk.cubes[neighbourIndex] == 0)
                        {
                            
                            CreateBlock(chunk, index);
                            break;
                        }
                    }

                }
            }
        }
        public void DespawnChunk(Chunk chunk)
        {
            if(chunk == null) return;
            foreach (int blockIndex in chunk.showedNodes)
            {
                DestroyBlock(chunk, blockIndex);
            }
        }
        
        private Vector3Int GetPositionInChunk(int index)
        {
            //16 width (X)
            //256 height (Y)
            //16 depth (Z)
            int x = index & 15;
            int y = (index >> 4) & 255;
            int z = index >> 12;
            return new Vector3Int(x, y, z);
        }
        private void CreateBlock(Chunk chunk, int index)
        {
            Vector3Int chunkOffset = new Vector3Int(chunk.Position.x << 4, 0, chunk.Position.y << 4);
            Vector3Int cubeWorldPosition = GetPositionInChunk(index) + chunkOffset;
            blockPool.SetCube(cubeWorldPosition, (Block)chunk.cubes[index]);
            chunk.showedNodes.Add(index);
        }   
        public bool DestroyBlock(Chunk chunk, int index)
        {

            Vector3Int chunkOffset = new Vector3Int(chunk.Position.x << 4, 0, chunk.Position.y << 4);
            Vector3Int cubeWorldPosition = GetPositionInChunk(index) + chunkOffset;
            if(blockPool.DisableCube(cubeWorldPosition))
            {                
                return true;
            }
            return false;
        }      
        
        internal void UpdateNeighbours(Chunk chunk, int index)
        {
            foreach (int neighbourIndex in neighboursLookup[index])
            {
                //empty nodes have got no visual (database[0] is empty)
                int neighbourBlockDatabaseID = chunk.cubes[neighbourIndex];
                Debug.Log("this block have data: " + neighbourBlockDatabaseID);
                if (neighbourBlockDatabaseID == 0) continue;
                UpdateVisual(chunk, neighbourIndex);
            }
        }

        private void UpdateVisual(Chunk chunk, int index)
        {
            foreach (int neighbourIndex in neighboursLookup[index])
            {
                //this is neighbours of neighbours
                //one of the neighbours is empty and this cube must be visible
                if (chunk.cubes[neighbourIndex] == 0)
                {
                    int idBlock = chunk.cubes[index];
                    Vector3Int neighbourWorldPosition = GetPositionInChunk(index) + new Vector3Int(chunk.Position.x << 4, 0, chunk.Position.y << 4);
                    blockPool.SetCube(neighbourWorldPosition, (Block)idBlock);
                    Debug.Log("Setting block " + neighbourWorldPosition + " to " + idBlock + " update");
                    //this will break this neighbour(neighbour of main block)
                    return;
                }

            }
        }

        internal void SpawnChunk(Chunk chunk)
        {
            for (int index = 0; index < chunk.cubes.Length; index++)
                {
                    //check neighbours, if any of them is 0(empty, then this must be visible)
                    //skip empty nodes
                    if(chunk.cubes[index] == 0) continue;
                    foreach (int neighbourIndex in neighboursLookup[index])
                    {
                        if (chunk.cubes[neighbourIndex] == 0)
                        {
                            
                            CreateBlock(chunk, index);
                            break;
                        }
                    }

                }
        }
    }
    
}
