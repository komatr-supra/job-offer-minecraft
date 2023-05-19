using System.Collections;
using Character;
using UnityEngine;
using Map;

//this is main class control game flow
namespace Core
{
    public class GameManager : MonoBehaviour
    {
        #region  VARIABLES
        [SerializeField] private GameObject playerGameObject;
        [SerializeField] private LayerMask layerMaskForPlayerSpawn;
        private MapManager mapManager;
        private const float playerPositionCheckingTime = 2f;
        private Vector2Int playerChunkPosition;
        private int generatedRadius = 4;       
        #endregion

        public void StartWorld(int seed)
        {
            mapManager = new MapManager(seed, generatedRadius);            
            mapManager.StartMap(playerChunkPosition, CreatePlayer);
        }
        //this is for load scene, not best solution
        private void CreatePlayer()
        {
            Invoke("SpawnPlayer", 2f);
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
        //check if player change chunk, its for procedural generator
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