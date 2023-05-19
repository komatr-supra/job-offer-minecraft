using UnityEngine;
using Map;

//connection between player and world
//i dont know... dont like this solution...
namespace Character
{    
    public class PlayerBuildingLink
    {
        private MapManager mapManager;
        public PlayerBuildingLink(MapManager mapManager)
        {
            this.mapManager = mapManager;
        }
        public void StartDigging(Vector3Int worldPosition) => mapManager.StartDigging(worldPosition);        
        public void StopDigging() => mapManager.StopDigging();
        public bool PlaceBlock(RaycastHit raycast, BlocksSO block) => mapManager.TryPlaceBlock(raycast, block);
    }
}

