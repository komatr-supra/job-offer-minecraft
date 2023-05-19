using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Timers;
using Unity.Collections;
using Unity.Jobs;

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
            //get all chunks
            Vector2Int chunkPos = chunk.Position;
            Vector2Int leftChunkPos = new Vector2Int(-1, 0) + chunkPos;
            mapDataProvider.GetChunk(leftChunkPos, out Chunk chunkLeft);
            Vector2Int rightChunkPos = new Vector2Int(1, 0) + chunkPos;
            mapDataProvider.GetChunk(rightChunkPos, out Chunk chunkRight);
            Vector2Int frontChunkPos = new Vector2Int(0, -1) + chunkPos;
            mapDataProvider.GetChunk(frontChunkPos, out Chunk chunkFront);
            Vector2Int backChunkPos = new Vector2Int(0, 1) + chunkPos;
            mapDataProvider.GetChunk(backChunkPos, out Chunk chunkBack);
            
            NativeArray<uint> createdBlocks = new NativeArray<uint>(65536, Allocator.TempJob);
            var blockDataLeft = new NativeArray<ushort>(chunkLeft.cubes, Allocator.TempJob);
            var blockDataRight = new NativeArray<ushort>(chunkRight.cubes, Allocator.TempJob);
            var blockDataFront = new NativeArray<ushort>(chunkFront.cubes, Allocator.TempJob);
            var blockDataBack = new NativeArray<ushort>(chunkBack.cubes, Allocator.TempJob);
            NativeArray<int> neigh = new NativeArray<int>(mapDataProvider.neighboursLookupDataArray,Allocator.TempJob);
            var blockDataMain = new NativeArray<ushort>(chunk.cubes, Allocator.TempJob);
            ChunkCreateJob chunkCreateJob = new ChunkCreateJob()
            {
                blockDataMain = blockDataMain,
                neighboursLookupDataArray = neigh,
                blockDataLeft = blockDataLeft,
                blockDataRight = blockDataRight,
                blockDataFront = blockDataFront,
                blockDataBack = blockDataBack,
                usedBlocks = createdBlocks
            };
            JobHandle job = chunkCreateJob.Schedule(65536, 64);
            job.Complete();

            var filledCubes = createdBlocks.Where(x => (x & 65535) != 0).ToArray();//just block data 0 is nothing
            //you should learn hex...
            blockPool.SetCubeAsync(filledCubes, chunk.Position);
            foreach (var item in filledCubes)
            {
                uint index = item >> 16;
                chunk.showedNodes.Add((ushort)index);
            }
            blockDataBack.Dispose();
            blockDataFront.Dispose();
            blockDataLeft.Dispose();
            blockDataRight.Dispose();
            createdBlocks.Dispose();
            neigh.Dispose();
            blockDataMain.Dispose();
        }
        public void CreateBlock(Vector3Int worldPosition, BlocksSO block)
        {
            blockPool.SetCube(worldPosition, block);
            //int signBitMaskX = worldPosition.x >> 31;
            //int signBitMaskZ = worldPosition.z >> 31;
            Vector2Int mappos = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);//((worldPosition.x & 15) | signBitMaskX, (worldPosition.z & 15) | signBitMaskZ);
            if(mapDataProvider.GetChunk(mappos, out Chunk chunk))
            {
                int realXWithOffset = worldPosition.x - (mappos.x << 4);
                int realZWithOffset = worldPosition.z - (mappos.y << 4);
                int neighbour1DIndexInHisChunk = realXWithOffset & 15 | ((worldPosition.y) << 4) | ((realZWithOffset & 255) << 12);
                if(chunk.showedNodes.Contains((ushort)neighbour1DIndexInHisChunk))
                {
                    return;
                }
                chunk.showedNodes.Add((ushort)neighbour1DIndexInHisChunk);
            }        
        }   
        public void UpdateNeighbours(Vector3Int worldPosition)
        {
            //this look like as a good place for save
            BlockData blockData = mapDataProvider.GetBlockDatas(worldPosition);
            mapDataProvider.GetChunk(blockData.mapPosition, out var chunk);
            //32-16 index 0-15 block type
            uint data = (uint)blockData.index1D << 16 | (uint)chunk.cubes[blockData.index1D];
            chunk.changedNodesData.Add(data);
            BlockData[] blockDatas = mapDataProvider.GetNeighbourDatas(worldPosition).ToArray();
            //each neighbour of the placed block
            foreach (var neighbour in blockDatas)
            {
                //is neighbour replaceable? -> free
                if(neighbour.blockSO.isReplaceable) continue;
                foreach (var neighbourNeighbour in mapDataProvider.GetNeighbourDatas(neighbour.worldPosition))
                {
                    if(neighbourNeighbour.blockSO.isReplaceable) 
                    {
                        blockPool.SetCube(neighbour.worldPosition, neighbour.blockSO);
                        break;
                    }
                    blockPool.DisableCube(neighbour.worldPosition);
                }
            }
        }
        public bool DestroyBlock(Vector3Int worldPosition)
        {
            if(!blockPool.DisableCube(worldPosition)) return false; 
            Vector2Int mappos = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            if(mapDataProvider.GetChunk(mappos, out Chunk chunk))
            {
                int realXWithOffset = worldPosition.x - (mappos.x << 4);
                int realZWithOffset = worldPosition.z - (mappos.y << 4);
                int neighbour1DIndexInHisChunk = realXWithOffset & 15 | (worldPosition.y << 4) | ((realZWithOffset & 255) << 12);
                if(chunk.showedNodes.Contains((ushort)neighbour1DIndexInHisChunk))
                {
                    chunk.showedNodes.Remove((ushort)neighbour1DIndexInHisChunk);
                }
            }
            UpdateNeighbours(worldPosition);
            return true;  
        }
        public void DespawnChunk(Chunk chunk)
        {
            //save changes
            FakeSaveSystem.Instance.SaveData(chunk.Position, chunk.changedNodesData.ToArray());
            List<Vector3Int> positionToDestroy = new();
            foreach (var item in chunk.showedNodes)
            {
                positionToDestroy.Add(new Vector3Int(chunk.Position.x << 4, 0, chunk.Position.y << 4) + MapDataProvider.GetPositionInChunk(item));                
            }
            blockPool.DisableCubeAsync(positionToDestroy);
        }
        public GameObject GetCubeGameObject(Vector3Int worldPosition)
        {
            return blockPool.GetGameObject(worldPosition);
        }
    }
    
}
