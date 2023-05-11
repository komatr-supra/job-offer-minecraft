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
        private Dictionary<Vector2Int, Chunk> activeChunks;
        private Digger digger;
        
        public MapManager(int seed, int mapDrawRadius)
        {
            activeChunks = new();
            this.mapDrawRadius = mapDrawRadius;
            //seed handle its basicly map start position in perlin noise => map is  done
            //save only changes?
            if(seed == -1) seed = (int)(System.DateTime.Now.Ticks);
            UnityEngine.Random.InitState(seed);
            //Debug.Log(seed);
            this.seed = seed;

            //prepare 
            mapGenerator = new MapGenerator();
            worldConstructor = new WorldConstructor();
            chunkGenerator = new ChunkGenerator();
            digger = new Digger();

        }
        private void CreateChunks(IEnumerable<Vector2Int> pos)
        {
            Chunk[] chunksToCreate = chunkGenerator.GenerateChunk(mapGenerator.GetMapDatas(pos)).ToArray();
            foreach (var item in chunksToCreate)
            {
                activeChunks.Add(item.Position, item);
            }        
            worldConstructor.SpawnChunks(chunksToCreate);
        }
        //this is not the best place?
        public void StartDigging(Vector3Int worldPosition)
        {
            float digTime = GetBlockSO(worldPosition).minigTime;
            digger.StartDigging(digTime, () => DestroyBlock(worldPosition));
        }
        private void DestroyBlock(Vector3Int worldPosition)
        {
            Vector2Int mapPosition = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            Chunk chunk = GetChunk(mapPosition);                    
            int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
           
            worldConstructor.DestroyBlock(chunk, index);
            SetCubeDataInChunk(worldPosition, 0);
        }
        public void StopDigging()
        {
            digger.StopDigging();
        }
        private BlocksSO GetBlockSO(Vector3Int worldPosition)
        {
            Vector2Int mapPosition = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            Chunk chunk = GetChunk(mapPosition);
            if(chunk == null)
            {
                Debug.Log("empty chunk in getblockso world pos: " + worldPosition + " map position: " + mapPosition);
            }            
            int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
            int databaseIndex = chunk.cubes[index];
            return FakeDatabase.Instance.GetBlock((Block)databaseIndex);
        }

        private Chunk GetChunk(Vector2Int mapPosition)
        {
            Chunk chunk = null;
            if(!activeChunks.TryGetValue(mapPosition, out chunk))            
                Debug.Log("you want change non active chunk " + mapPosition);
            return chunk;
        }

        //player is on another chunk, change map
        internal void StartMap(Vector2Int playerChunkPosition)
        {
            Vector2Int[] newChunksMapPositions = GetUsedMapPositions(playerChunkPosition);
            //compare new and old position
            //if is in old and not in new, destroy
            //if is in new and not in old, create

            //--------------------------------------------TEST just for strat---------------------------------------------
            CreateChunks(newChunksMapPositions);

        }

        private Vector2Int[] GetUsedMapPositions(Vector2Int playerChunkPosition)
        {
            List<Vector2Int> activeMapsPositions = new();
            for (int xx = -mapDrawRadius; xx <= mapDrawRadius; xx++)
            {
                for (int yy = -mapDrawRadius; yy <= mapDrawRadius; yy++)
                {
                    activeMapsPositions.Add(new Vector2Int(playerChunkPosition.x + xx, playerChunkPosition.y + yy));
                }
            }
            return activeMapsPositions.ToArray();
        }
        public bool TryPlaceBlock(RaycastHit raycast, Block block)
        {

            return true;
        }
        private void SetCubeDataInChunk(Vector3Int worldPosition, int blockIndexInDatabase)
        {
            Vector2Int mapPosition = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            Chunk chunk = GetChunk(mapPosition);                    
            int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
            chunk.cubes[index] = blockIndexInDatabase;
            //update neighbours
            worldConstructor.UpdateNeighbours(chunk, index);

        }
        public void PlayerMoved(Vector2Int previousPosition, Vector2Int newPosition)
        {
            Vector2Int offset = newPosition - previousPosition;

            int signX = Mathf.Sign(offset.x).ToInt();
            int signY = Mathf.Sign(offset.y).ToInt();

            for (int x = 0; x < Mathf.Abs(offset.x); x++)
            {
                int deltedX = previousPosition.x - (signX * mapDrawRadius);
                deltedX -= (x * signX);
                for (int yy = -mapDrawRadius; yy <= mapDrawRadius; yy++)
                {
                    var chunk = GetChunk(new Vector2Int(deltedX,yy));
                    worldConstructor.DespawnChunk(chunk);
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int deletedY = previousPosition.y - (signY * mapDrawRadius);
                deletedY -= (signX * y);
                for (int xx = -mapDrawRadius; xx <= mapDrawRadius; xx++)
                {
                    var chunk = GetChunk(new Vector2Int(xx, deletedY));
                    worldConstructor.DespawnChunk(chunk);
                }
            }
            for (int x = 0; x < Mathf.Abs(offset.x); x++)
            {
                int loadedX = previousPosition.x + (signX * mapDrawRadius);
                loadedX += (x * signX);
                for (int yy = -mapDrawRadius; yy <= mapDrawRadius; yy++)
                {
                    Vector2Int[] mapPosition = {new Vector2Int(loadedX,yy)};
                    CreateChunks(mapPosition);
                    //var chunk = chunkGenerator.GenerateChunk(mapGenerator.GetMapDatas(mapPosition)).ToArray();
                    //worldConstructor.SpawnChunks(chunk);
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int loadedY = previousPosition.y + (signY * mapDrawRadius);
                loadedY += (y * signY);
                for (int xx = -mapDrawRadius; xx <= mapDrawRadius; xx++)
                {
                    Vector2Int[] mapPosition = {new Vector2Int(xx, loadedY)};

                    CreateChunks(mapPosition);
                    //var chunk = chunkGenerator.GenerateChunk(mapGenerator.GetMapDatas(mapPosition)).ToArray();
                    //worldConstructor.SpawnChunks(chunk);
                }
            }
        }

    }
}
