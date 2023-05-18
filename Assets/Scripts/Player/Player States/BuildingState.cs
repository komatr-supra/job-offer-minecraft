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
        private Inventory inv;           //TEST
        public BuildingState(Player player, PlayerBuildingLink buildingLink, Inventory inventory)
        {
            this.player = player;
            this.buildingLink = buildingLink;
            inv = inventory;
        }
        public void OnEnter()
        {
            Debug.Log("building state");
            if(inv == null)Debug.Log("NO INV");
            player.MakeRaycast(out RaycastHit raycast);
            if(!inv.TryGetSelected(out var blocksSO))
            {
                Debug.Log("cant find block by selection");
                return;
            }
            else
            {
                Debug.Log("block found");
            }
            if(buildingLink.PlaceBlock(raycast, blocksSO))
            {
                Debug.Log("placing block");
                inv.RemoveBlockFromInventory(blocksSO);
            }
            else
            {
                Debug.Log("block not placed");
            }
        }

        public void OnExit()
        {
            
        }

        public void Tick()
        {

        }
    }
}