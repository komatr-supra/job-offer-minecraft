using UnityEngine;

//players digging
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
            //no real tick, maybe should be tick here...
        }
        public void OnEnter()
        {
            if(player.MakeRaycast(out RaycastHit raycast))
            {
                Vector3Int blockPosition = raycast.collider.transform.position.ToVec3Int();
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

