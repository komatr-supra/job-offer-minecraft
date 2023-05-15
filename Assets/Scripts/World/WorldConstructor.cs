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
            //Debug.Log("spawning chunk");
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
                    int neighbourIndex = (neighbourData & 65535);
                    //Debug.Log("neighbour");
                    int chunkIndex = (neighbourData >> 16);
                    chunkIndex = chunkIndex & 7;
                    if(chunkIndex == 5) continue;
                    //if(chunkIndex < 0) {Debug.Log("mimo" + chunkIndex); }
                    //if(chunkIndex > 5) Debug.Log("hovno" + neighbourIndex + " " + neighbourData + " " + chunkIndex);
                    var v = chunks[chunkIndex];
                    int blockData = v.cubes[neighbourIndex];
                    if (blockData == 0)
                    {
                        CreateBlock(chunks[0].Position, index, (Block)chunks[0].cubes[index]);
                        break;
                    }
                }
                
            }
            
        }

        private void CreateBlock(Vector2Int position, int index, Block block)
        {
            Vector3Int worldPos = new Vector3Int(position.x << 4, 0, position.y << 4) + mapDataProvider.GetPositionInChunk(index);
            //if(worldPos.x < 0 || worldPos.z < 0) Debug.Log("creating block" + worldPos);
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
            //int signBitMaskX = worldPosition.x >> 31;
            //int signBitMaskZ = worldPosition.z >> 31;
            Vector2Int mappos = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);//((worldPosition.x & 15) | signBitMaskX, (worldPosition.z & 15) | signBitMaskZ);
            if(mapDataProvider.GetChunk(mappos, out Chunk chunk))
            {
                int realXWithOffset = worldPosition.x - (mappos.x * 16);// mappos.x * 16 (-16) +
                int realZWithOffset = worldPosition.z - (mappos.y * 16);
                int neighbour1DIndexInHisChunk = realXWithOffset & 15 | ((worldPosition.y) << 4) | ((realZWithOffset & 255) << 12);
                //var pos = new Vector3Int(chunk.Position.x << 4, 0, chunk.Position.y << 4) + mapDataProvider.GetPositionInChunk(item);
               
                if(chunk.showedNodes.Contains(neighbour1DIndexInHisChunk))
                {
                    //Debug.Log(realXWithOffset + "is existing id:" + neighbour1DIndexInHisChunk);
                    return;
                }
                chunk.showedNodes.Add(neighbour1DIndexInHisChunk);
                //Debug.Log("creating " + neighbour1DIndexInHisChunk + " as showed nodes");
            }        
        }   
        private void CreateBlock(Vector3Int worldPosition, BlocksSO blockSO)
        {
            blockPool.SetCube(worldPosition, blockSO);
            UpdateChunkData(worldPosition);
        }

        private void UpdateChunkData(Vector3Int worldPosition)
        {
            Vector2Int mappos = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            if (mapDataProvider.GetChunk(mappos, out Chunk chunk))
            {
                int realXWithOffset = worldPosition.x - (mappos.x * 16);// mappos.x * 16 (-16) +
                int realZWithOffset = worldPosition.z - (mappos.y * 16);
                int neighbour1DIndexInHisChunk = realXWithOffset & 15 | (worldPosition.y << 4) | ((realZWithOffset & 15) << 12);
                //int neighbour1DIndexInHisChunk = worldPosition.x & 15 | (worldPosition.y << 4) | ((worldPosition.z & 15 )<< 12);
                if (chunk.showedNodes.Contains(neighbour1DIndexInHisChunk)) return;
                chunk.showedNodes.Add(neighbour1DIndexInHisChunk);

            }
        }

        public void UpdateNeighbours(Vector3Int worldPosition)
        {
            BlockData[] blockDatas = mapDataProvider.GetNeighbourDatas(worldPosition).ToArray();
            //each neighbour of the placed block
            foreach (var neighbour in blockDatas)
            {
                //this mean air or water next to the placed block
                //... its not best, what about glass???? next variable isTransparent?
                //whatever nothing is changed for anyone
                if(neighbour.blockSO.isReplaceable) continue;
                //each neighbour should calculate if something changed(neighbour of this neighbour - its include placed block)
                bool isVisible = false;
                foreach (var neighbourNeighbour in mapDataProvider.GetNeighbourDatas(neighbour.worldPosition))
                {
                    //if any of the neighbour is replaceable, then this is visible
                    if(neighbourNeighbour.blockSO.isReplaceable)
                    {
                        isVisible = true;
                        break;
                    }
                }
                //set this block visibility
                if(isVisible)
                {
                    
                    blockPool.SetCube(neighbour.worldPosition, neighbour.blockSO);
                    //UpdateChunkData(worldPosition);
                }
                else
                {
                    //blockPool.DisableCube(neighbour.worldPosition);
                    DestroyBlock(neighbour.worldPosition);
                }
            }
        }
        public bool DestroyBlock(Vector3Int worldPosition)
        {
            if(!blockPool.DisableCube(worldPosition)) return false; 
            Vector2Int mappos = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            if(mapDataProvider.GetChunk(mappos, out Chunk chunk))
            {
                int realXWithOffset = worldPosition.x - (mappos.x * 16);// mappos.x * 16 (-16) +
                int realZWithOffset = worldPosition.z - (mappos.y * 16);
                int neighbour1DIndexInHisChunk = realXWithOffset & 15 | (worldPosition.y << 4) | ((realZWithOffset & 15) << 12);
                //int neighbour1DIndexInHisChunk = worldPosition.x & 15 | (worldPosition.y << 4) | ((worldPosition.z & 15) << 12);
                if(chunk.showedNodes.Contains(neighbour1DIndexInHisChunk))
                {
                    chunk.showedNodes.Remove(neighbour1DIndexInHisChunk);
                }
            }
            UpdateNeighbours(worldPosition);
            return true;  
        }

        internal void DespawnChunk(Chunk chunk)
        {
            
            //Debug.Log("disabling chunk at position " + chunk.Position* 16);
            foreach (var item in chunk.showedNodes)
            {
                var pos = new Vector3Int(chunk.Position.x << 4, 0, chunk.Position.y << 4) + mapDataProvider.GetPositionInChunk(item);
                Debug.Log(pos + " was deleted from " + chunk.Position* 16);
                blockPool.DisableCube(pos);
            }
        }
    }
    
}
