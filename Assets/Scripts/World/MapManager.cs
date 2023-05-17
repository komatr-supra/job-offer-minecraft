using System;
using UnityEngine;

//this is main map manager, IO entry
namespace Map
{
    public class MapManager
    {
        #region VARIABLES
        private int seed;
        private WorldConstructor worldConstructor;
        private MapProcedural mapProcedural;
        private MapDataProvider mapDataProvider;
        private Digger digger;
        //this is ugly
        private GameObject selectedCube;
        #endregion

        #region CONSTRUCTOR
        public MapManager(int seed, int mapDrawRadius)
        {            
            //seed handle its basicly map start position in perlin noise => map is  done
            //save only changes?
            //TODO this was changed in UI
            if(seed == -1) seed = (int)(System.DateTime.Now.Ticks);
            UnityEngine.Random.InitState(seed);
            //this is for save -> recreate the perlin noise
            this.seed = seed;

            mapDataProvider = new MapDataProvider(mapDrawRadius);
            worldConstructor = new WorldConstructor(mapDataProvider);
            mapProcedural = new MapProcedural(mapDataProvider, worldConstructor);
            digger = new Digger();
        }
        #endregion

        #region FOR GAME MANAGER
        // called at the start of the game
        public void StartMap(Vector2Int positon, Action onComplete)
        {
            mapProcedural.StartMap(positon, onComplete);
        }

        //when player was moved to another chunk
        public void PlayerMoved(Vector2Int previousPosition, Vector2Int newPosition)
        {
            mapProcedural.PlayerMoved(previousPosition, newPosition);
        }
        #endregion

        #region  PLAYER ACTIONS
        //this is an action for player
        public void StartDigging(Vector3Int worldPosition)
        {
            BlockData blockData = mapDataProvider.GetBlockDatas(worldPosition);
            selectedCube = worldConstructor.GetCubeGameObject(worldPosition);
            float digTime = blockData.blockSO.minigTime;
            digger.StartDigging(digTime, () => DestroyBlock(worldPosition));
            selectedCube.GetComponent<MeshRenderer>().material.color = Color.gray;
        }

        //just callback method for end of digging... 
        private bool DestroyBlock(Vector3Int worldPosition)
        {
            //bottom limit... looks like digging, but no action at the end is changed
            if(worldPosition.y == 0) return false;
            //set data(clear)
            if(mapDataProvider.SetBlockData(worldPosition, Block.none))
            {   
                selectedCube.GetComponent<MeshRenderer>().material.color = Color.white;
                return worldConstructor.DestroyBlock(worldPosition);
            }            
            return false;
        }

        //player action, to cancel digging
        public void StopDigging()
        {
            if(selectedCube != null)
            {
                selectedCube.GetComponent<MeshRenderer>().material.color = Color.white;
            }
            digger.StopDigging();
        }

        //player action build block
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
        #endregion
    }
}