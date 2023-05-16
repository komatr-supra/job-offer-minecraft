using System.Collections;
using Character;
using UnityEngine;
using Map;

//this is main class control game flow
namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerGameObject;
        [SerializeField] private LayerMask layerMaskForPlayerSpawn;
        private MapManager mapManager;
        private const float playerPositionCheckingTime = 2f;
        private Vector2Int playerChunkPosition;
        private int generatedRadius = 4;
       
        public void StartWorld(int seed)
        {
            mapManager = new MapManager(seed, generatedRadius);            
            mapManager.StartMap(playerChunkPosition, SpawnPlayer);
        }
        private void SpawnPlayer()
        {            
            Vector3 startPoint = Vector3.zero;
            Physics.Raycast(new Vector3(0,300,0), Vector3.down, out RaycastHit raycast, 301f, layerMaskForPlayerSpawn);            
            if(raycast.collider) startPoint = raycast.point;
            playerGameObject.transform.position = startPoint;
            var player = playerGameObject.GetComponent<Player>();
            player.Init(new PlayerBuildingLink(mapManager));
            playerGameObject.SetActive(true);
            playerChunkPosition = playerGameObject.transform.position.ChunkPos();

            StartCoroutine(CheckPlayerChunkPosition());
        }
        private IEnumerator CheckPlayerChunkPosition()
        {
            var wait = new WaitForSeconds(playerPositionCheckingTime);
            while (true)
            {
                Vector2Int newPlayerChunkPOsition = playerGameObject.transform.position.ChunkPos();
                if(newPlayerChunkPOsition != playerChunkPosition)
                {
                    //Debug.Log("player change chunk");
                    mapManager.PlayerMoved(playerChunkPosition, newPlayerChunkPOsition);
                    playerChunkPosition = newPlayerChunkPOsition;
                }
                yield return wait;
            }
        }
    }
}