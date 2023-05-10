using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;
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
        public bool PlaceBlock(RaycastHit raycast, Block block) => mapManager.TryPlaceBlock(raycast, block);
    }
}

