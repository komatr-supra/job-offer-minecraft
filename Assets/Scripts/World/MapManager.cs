using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Map
{
    public class MapManager
    {
        private int seed;
        private int mapDrawRadius;                    // this is +- value (its double size square) ans should be in setup
        private ChunkGenerator chunkGenerator;
        private MapGenerator mapGenerator;
        private WorldConstructor worldConstructor;        
        private List<Chunk> activeChunks;
        private Digger digger;
        
        public MapManager(int seed, int mapDrawRadius)
        {
            activeChunks = new();
            this.mapDrawRadius = mapDrawRadius;
            //seed handle its basicly map start position in perlin noise => default state
            //save only changes?
            if(seed == -1) seed = (int)(System.DateTime.Now.Ticks);
            UnityEngine.Random.InitState(seed);
            Debug.Log(seed);
            this.seed = seed;

            //prepare 
            mapGenerator = new MapGenerator();
            worldConstructor = new WorldConstructor();
            chunkGenerator = new ChunkGenerator();
            digger = new Digger();

        }
        private void UpdateChunks(IEnumerable<Vector2Int> pos)
        {
            Chunk[] chunksToCreate = chunkGenerator.GenerateChunk(mapGenerator.GetMapDatas(pos)).ToArray();    
            foreach (var item in chunksToCreate)
            {
                activeChunks.Add(item);
            }        
            worldConstructor.SpawnChunks(chunksToCreate);
        }
        //this is not the best place?
        public void StartDigging(Vector3Int worldPosition)
        {
            float digTime = GetBlockSO(worldPosition).minigTime;
            digger.StartDigging(digTime, () => worldConstructor.DestroyBlock(worldPosition));
        }
        public void StopDigging()
        {
            digger.StopDigging();
        }
        private BlocksSO GetBlockSO(Vector3Int worldPosition)
        {
            Vector2Int mapPosition = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            Chunk chunk = GetChunk(mapPosition);                    
            int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
            int databaseIndex = chunk.cubes[index];
            return FakeDatabase.Instance.GetBlock((Block)databaseIndex);
        }

        private Chunk GetChunk(Vector2Int mapPosition)
        {
            foreach (var chunk in activeChunks)
            {
                if(chunk.Position == mapPosition) return chunk;
            }
            Debug.LogError("you want change non active chunk " + mapPosition);
            return default;
        }

        //player is on another chunk, change map
        internal void PlayerChunkChanged(Vector2Int playerChunkPosition)
        {
            Vector2Int[] newChunksMapPositions = GetUsedMapPositions(playerChunkPosition);
            //compare new and old position
            //if is in old and not in new, destroy
            //if is in new and not in old, create

            //--------------------------------------------TEST just for strat---------------------------------------------
            UpdateChunks(newChunksMapPositions);

        }

        private Vector2Int[] GetUsedMapPositions(Vector2Int playerChunkPosition)
        {
            List<Vector2Int> activeMapsPositions = new();
            for (int xx = 0; xx < mapDrawRadius; xx++)
            {
                for (int yy = 0; yy < mapDrawRadius; yy++)
                {
                    activeMapsPositions.Add(new Vector2Int(playerChunkPosition.x + xx, playerChunkPosition.y + yy));
                }
            }
            return activeMapsPositions.ToArray();
        }

        public void RecalculateActiveChunks(Vector2Int newChunkPosition)
        {
            Debug.Log("chunks change");
        }
        public bool TryPlaceBlock(RaycastHit raycast, Block block)
        {

            return true;
        }

    }
}
