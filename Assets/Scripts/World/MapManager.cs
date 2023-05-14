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
        private ChunkGenerator chunkGenerator;
        private MapGenerator mapGenerator;
        private WorldConstructor worldConstructor;
        private MapProcedural mapProcedural;
        private MapDataProvider mapDataProvider;
        private Digger digger;
        
        public MapManager(int seed, int mapDrawRadius)
        {            
            //seed handle its basicly map start position in perlin noise => map is  done
            //save only changes?
            if(seed == -1) seed = (int)(System.DateTime.Now.Ticks);
            UnityEngine.Random.InitState(seed);
            //Debug.Log(seed);
            this.seed = seed;

            //prepare 
            mapDataProvider = new MapDataProvider(mapDrawRadius);
            worldConstructor = new WorldConstructor(mapDataProvider);
            mapProcedural = new MapProcedural(mapDataProvider, worldConstructor);
            digger = new Digger();

        }
        public void StartMap(Vector2Int positon)
        {
            mapProcedural.StartMap(positon);
        }
        public void PlayerMoved(Vector2Int previousPosition, Vector2Int newPosition)
        {
            mapProcedural.PlayerMoved(previousPosition, newPosition);
        }
        public void StartDigging(Vector3Int worldPosition)
        {
            BlockData blockData = mapDataProvider.GetBlockDatas(worldPosition);
            float digTime = blockData.blockSO.minigTime;
            digger.StartDigging(digTime, () => DestroyBlock(worldPosition));
        }
        private bool DestroyBlock(Vector3Int worldPosition)
        {
            if(worldPosition.y == 0) return false;
            if(mapDataProvider.SetBlockData(worldPosition, Block.none))
            {   
                return worldConstructor.DestroyBlock(worldPosition);
            }            
            return false;
        }        
        public void StopDigging()
        {
            digger.StopDigging();
        }
        

        
        public bool TryPlaceBlock(RaycastHit raycast, Block block)
        {
            //height build restrict5ion
            if(raycast.point.y > 30) return false;
            //world cube targeted cube
            Vector3Int worldPosition = raycast.collider.transform.position.ToVec3Int();
            //world hited point
            Vector3 worldHitPoint = raycast.point;
            float minDistance = float.MaxValue;
            BlockData bestNeighbour = new();
            foreach (BlockData neighbourBlock in mapDataProvider.GetNeighbourDatas(worldPosition))
            {
                if(!neighbourBlock.blockSO.isReplaceable) continue;
                
                float currentBlockDistance = Vector3.Distance(neighbourBlock.worldPosition, worldHitPoint);
                if(currentBlockDistance < minDistance)
                {
                    bestNeighbour = neighbourBlock;
                    minDistance = currentBlockDistance;
                }
            }
            //just save check
            if(minDistance == float.MaxValue) return false;            
            //check if place is clear
            if(Physics.CheckBox(bestNeighbour.worldPosition, Vector3.one * 0.49f, Quaternion.identity))
            {
                Debug.Log("place is occupied");
                return false;
            } 
            worldConstructor.CreateBlock(bestNeighbour.worldPosition, block);
            mapDataProvider.SetBlockData(bestNeighbour.worldPosition, block);
            worldConstructor.UpdateNeighbours(bestNeighbour.worldPosition);
            return true;
        }
        
        

    }
}
