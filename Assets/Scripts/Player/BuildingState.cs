using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Character.State
{    
    public class BuildingState : IState
    {
        private PlayerBuildingLink buildingLink;
        private Player player;
        //block to use -> from inventory
        private Block blockToPlace = Block.Dirt;                //TEST
        public BuildingState(Player player, PlayerBuildingLink buildingLink)
        {
            this.player = player;
            this.buildingLink = buildingLink;
        }
        public void OnEnter()
        {
            player.MakeRaycast(out RaycastHit raycast);
            buildingLink.PlaceBlock(raycast, blockToPlace);
        }

        public void OnExit()
        {
            
        }

        public void Tick()
        {

        }
    }
}