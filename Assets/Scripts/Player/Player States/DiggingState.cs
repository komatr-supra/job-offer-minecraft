using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Character.State
{
    
    public class DiggingState : IState
    {
        private Player player;
        private PlayerBuildingLink buildingLink;        
        public DiggingState(Player player, PlayerBuildingLink buildingLink)
        {
            this.player = player;
            this.buildingLink = buildingLink;
        }
        public void Tick()
        {
            //show dig state

        }

        public void OnEnter()
        {
            Debug.Log("dig state start");
            if(player.MakeRaycast(out RaycastHit raycast))
            {
                Vector3Int blockPosition = raycast.collider.transform.position.ToVec3Int();
                //Debug.Log(buildingLink);
                buildingLink.StartDigging(blockPosition);
            }

            player.onSelectedChange += ResetMining;
        }

        public void OnExit()
        {
            player.onSelectedChange -= ResetMining;
            buildingLink.StopDigging();
        }
        private void ResetMining()
        {
            buildingLink.StopDigging();
            if(player.SelectedTransform == null) return;
            Vector3Int blockPOsition = player.SelectedTransform.position.ToVec3Int();
            buildingLink.StartDigging(blockPOsition);
        }
            
    }
}

