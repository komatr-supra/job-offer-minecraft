using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using Map;
namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerObject;
        [SerializeField] private LayerMask layerMaskForPlayerSpawn;
        [SerializeField] private int generatedRadius = 4;
        private const float playerPositionCheckingTime = 1f;
        private MapManager mapManager;
        private Vector2Int playerChunkPosition;
        
        bool init;
        private void Start() {
            StartWorld();
        }
        public void StartWorld(int seed = -1)
        {   
            mapManager = new MapManager(seed, generatedRadius);
            //set player
            mapManager.StartMap(playerChunkPosition);
            Vector3 startPoint = Vector3.zero;
            Physics.Raycast(new Vector3(16,300,16), Vector3.down, out RaycastHit raycast, 301f, layerMaskForPlayerSpawn);            
            if(raycast.collider) startPoint = raycast.point;
            else Debug.Log("no ground found");
            playerObject.transform.position = startPoint;
            var player = playerObject.GetComponent<Player>();
            player.Init(new PlayerBuildingLink(mapManager));
            playerObject.SetActive(true);
            //get Position for map generator
            playerChunkPosition = playerObject.transform.position.ChunkPos();

            StartCoroutine(CheckPlayerChunkPosition());
            
        }
        private IEnumerator CheckPlayerChunkPosition()
        {
            var wait = new WaitForSeconds(playerPositionCheckingTime);
            while (true)
            {
                Vector2Int newPlayerChunkPOsition = playerObject.transform.position.ChunkPos();
                if(newPlayerChunkPOsition != playerChunkPosition)
                {
                    mapManager.PlayerMoved(playerChunkPosition, newPlayerChunkPOsition);
                    playerChunkPosition = newPlayerChunkPOsition;
                }
                yield return wait;
            }
        }
        
    }
}