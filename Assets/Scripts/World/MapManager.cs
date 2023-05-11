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
                if(activeChunks.TryAdd(item.Position, item))
                {
                    Debug.Log("chunk " + item.Position + " is loading");
                    worldConstructor.SpawnChunks(chunksToCreate);
                }
                else
                {
                    Debug.Log("chunk " + item.Position + " is loaded and dont need make it");
                }
            }        
        }
        //this is not the best place?
        public void StartDigging(Vector3Int worldPosition)
        {
            float digTime = GetBlockSO(worldPosition).minigTime;
            digger.StartDigging(digTime, () => DestroyBlock(worldPosition));
        }
        private bool DestroyBlock(Vector3Int worldPosition)
        {
            if(worldPosition.y == 0) return false;
            Vector2Int mapPosition = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
            Chunk chunk = GetChunk(mapPosition);                    
            int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
           
            if(worldConstructor.DestroyBlock(chunk, index))
            {
                SetCubeDataInChunk(worldPosition, 0);                
                return true;
            }
            return false;
        }        
        public void StopDigging()
        {
            digger.StopDigging();
        }
        private BlocksSO GetBlockSO(Vector3Int worldPosition)
        {
            Vector2Int mapPosition = new Vector2Int(worldPosition.x / 16, worldPosition.z / 16);
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
            if(activeChunks.TryGetValue(mapPosition, out chunk))            
            return chunk;
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
            //height build restrict5ion
            if(raycast.point.y > 30) return false;
            //world cube targeted cube
            Vector3Int worldPosition = raycast.collider.transform.position.ToVec3Int();
            //world hited point
            Vector3 worldHitPoint = raycast.point;
            //get neighbours           
            Vector2Int mapPosition = new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4); 
            Chunk chunk = GetChunk(mapPosition);
            int index = (worldPosition.x & 15) + (worldPosition.y << 4) + ((worldPosition.z & 15) << 12);
            var neighboursIndexes = worldConstructor.neighboursLookup[index];
            //find nearest
            List<Vector3Int> positions = new();
            foreach (int indexOfNeighbour in neighboursIndexes)
            {
                if(chunk.cubes[indexOfNeighbour] != 0) continue;
                var v = worldConstructor.GetPositionInChunk(indexOfNeighbour);
                v += new Vector3Int(mapPosition.x * 16, 0, mapPosition.y * 16);
                positions.Add(v);
                Debug.Log(v);
            }
            //nearest empty
            var near = positions.OrderBy(x => Vector3.Distance(x, worldHitPoint)).First();
            index = (near.x & 15) + (near.y << 4) + ((near.z & 15) << 12);
            Debug.Log("near selected: " + near);
            Debug.Log("position of hit" + worldHitPoint);
            worldConstructor.CreateBlock(chunk, index);
            SetCubeDataInChunk(near, 0);
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
                    activeChunks.Remove(new Vector2Int(deltedX,yy));
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
                    activeChunks.Remove(new Vector2Int(xx, deletedY));
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
