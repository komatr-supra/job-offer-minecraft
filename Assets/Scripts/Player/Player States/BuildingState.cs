using UnityEngine;
namespace Character.State
{
    public class BuildingState : IState
    {
        private PlayerBuildingLink buildingLink;
        private Player player;
        private Inventory inv;
        public BuildingState(Player player, PlayerBuildingLink buildingLink, Inventory inventory)
        {
            this.player = player;
            this.buildingLink = buildingLink;
            inv = inventory;
        }
        public void OnEnter()
        {
            player.MakeRaycast(out RaycastHit raycast);
            if(!inv.TryGetSelected(out var blocksSO)) return;
            if(buildingLink.PlaceBlock(raycast, blocksSO)) inv.RemoveBlockFromInventory(blocksSO);            
        }
        public void OnExit()
        {
            
        }
        public void Tick()
        {

        }
    }
}