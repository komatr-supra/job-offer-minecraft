using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask layerMaskForPlayerSpawn;
    private Action<Vector2Int> onCharacterPositionChanged;
    private MapManager mapManager;
    bool init;
    private void Start() {
        StartWorld();
    }
    public void StartWorld(int seed = -1)
    {   
        //prepare map
        mapManager = new MapManager(seed, onCharacterPositionChanged);
        mapManager.Generate(Vector2Int.zero);
        mapManager.Generate(Vector2Int.up);
        mapManager.Generate(Vector2Int.right);
        mapManager.Generate(Vector2Int.left);
        mapManager.Generate(Vector2Int.down);
        mapManager.Generate(new Vector2Int(1,1));
        mapManager.Generate(new Vector2Int(1,-1));
        mapManager.Generate(new Vector2Int(-1,1));
        mapManager.Generate(new Vector2Int(-1,-1));
    }
    private void Update() {
        if(!init)
        {
            init = true;
            //create player
            Vector3 startPoint = Vector3.zero;
            Physics.Raycast(new Vector3(8,300,8), Vector3.down, out RaycastHit raycast, 301f, layerMaskForPlayerSpawn);
            if(raycast.collider) startPoint = raycast.point;
            player.transform.position = startPoint;
            player.GetComponent<Builder>().SetBuildLink(new PlayerBuildLink(mapManager.GetWorldConstructor));
            player.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.G)) 
        if(Input.GetKeyDown(KeyCode.G)) mapManager.Generate(Vector2Int.up*2);
    }
    
}
