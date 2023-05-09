using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class Builder : MonoBehaviour
{
    [SerializeField] private Transform lookTransform;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float sphereCastRadius = 0.01f;
    [SerializeField] private float buildingDistance = 3f;
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private LayerMask layerMaskGround;
    private PlayerBuildLink playerBuildLink;
    private bool isBuildingMode;
    private bool onMainAction;
    private bool onSecondaryAction;
    private GameObject visual;
    private Transform selectedTransform;
    private Action onSelectedChange;
    private void Start()
    {
        visual = Instantiate(visualPrefab);
        visual.SetActive(false);
        isBuildingMode = true;
        //StartCoroutine(StartBuildCoroutine());
    }
    public void SetBuildLink(PlayerBuildLink playerBuildLink)
    {
        this.playerBuildLink = playerBuildLink;

    }
    public void OnMainAction(InputValue inputValue)
    {
        bool isRayHit = MakeRaycast(out var raycastHit);
        Vector3Int targetedCube = isRayHit ? raycastHit.collider.transform.position.ToVec3Int() : Vector3Int.zero;
        
        playerBuildLink.Dig(inputValue.isPressed && isRayHit, targetedCube, onSelectedChange);
        //if(!inputValue.isPressed) return;
        //if (!inputValue.isPressed || !MakeRaycast(out var raycastHit)) return;
        //Vector3Int hitBlockWorldPosition = 
        //playerBuildLink.Dig(hitBlockWorldPosition);
        //if(onMainAction) digCoroutine = StartCoroutine(DigCoroutine());

    }
    private void Update()
    {
        //check target change
        Transform newTransform = null;
        if(MakeRaycast(out var raycast)) newTransform = raycast.collider.transform;
        if(selectedTransform != newTransform)
        {
            Debug.Log("taget changed");
            selectedTransform = newTransform;
            onSelectedChange?.Invoke();
        }
        
    }
    //todo clean

    /*
    private IEnumerator DigCoroutine()
    {
        if(!GetCubePosition(out var cubePositionToDestroy)) yield break;
        float counter = 0;
        float timeToBreakCube = 1;//mapGenerator.GetBreakTime(cubePositionToDestroy);
        while(MakeRaycast(out var raycast) && onMainAction && !onSecondaryAction)
        {
            if(!MakeRaycast(out var raycast))
            {
                yield return null;
                continue;
            }
            if(cubePositionToDestroy != raycast.collider.transform.position.ToVec3Int())
            {
                cubePositionToDestroy = raycast.collider.transform.position.ToVec3Int();
                counter = 0;
                continue;
            }
            counter += Time.deltaTime;
            yield return null;
            if(counter > timeToBreakCube)
            {
                BreakBlock(cubePositionToDestroy);
                yield return null;
                //try next cube
                if(!GetCubePosition(out cubePositionToDestroy))
                {
                    Debug.Log("no more block to dig");
                    continue;
                }
                counter = 0;
                timeToBreakCube = mapGenerator.GetBreakTime(cubePositionToDestroy);
                continue;
                //yield break;
            }
        }
        
    }*/
    private bool GetCubePosition(out Vector3Int pos)
    {
        if(MakeRaycast(out var raycast))
        {
            pos = raycast.collider.transform.position.ToVec3Int();
            return true;
        }
        pos = new();
        return false;
    }
    private void BreakBlock(Vector3Int cubePosition)
    {
        //mapGenerator.BreakBlock(cubePosition);
    }

    public void OnSecondaryAction(InputValue inputValue)
    {
        if (!inputValue.isPressed || !MakeRaycast(out var raycastHit)) return;
        Vector3 pointOnBlock = raycastHit.collider.ClosestPointOnBounds(raycastHit.point);
        Vector3Int hitBlockWorldPosition = raycastHit.collider.transform.position.ToVec3Int();
        PlaceBlock(pointOnBlock, hitBlockWorldPosition, Block.Dirt);
    }
    public void OnBuild(InputValue inputValue)
    {
        if(inputValue.isPressed == false) return;
        isBuildingMode = !isBuildingMode;
        if(isBuildingMode)
        {
            //StartCoroutine(StartBuildCoroutine());
        }

    }
    private IEnumerator StartBuildCoroutine()
    {
        while (isBuildingMode)
        {
            RaycastHit raycastHit;
            if (!MakeRaycast(out raycastHit))
            {
                visual.SetActive(false);
            }
            else
            {
                Vector3 hitCubePoint = raycastHit.collider.transform.position;
                Vector3 pointToCompare = raycastHit.collider.ClosestPointOnBounds(raycastHit.point);
                List<Vector3> neighbourFreePosition = null;//mapGenerator.GetFreeNeighbourPosition(raycastHit.collider.transform.position.ToVec3Int());
                Vector3 visualPosition = neighbourFreePosition.OrderBy(vector => Vector3.Distance(pointToCompare, vector)).First();
                visual.transform.position = visualPosition;
                visual.SetActive(true);
                if (onSecondaryAction)
                {                    
                    
                    onSecondaryAction = false; 
                }
            }
            yield return null;
        }
        visual.SetActive(false);
    }

    private bool MakeRaycast(out RaycastHit raycastHit)
    {
        return Physics.SphereCast(lookTransform.position, sphereCastRadius, lookTransform.forward, out raycastHit, buildingDistance, layerMaskGround);
    }
    //todo inventory return bool
    private void PlaceBlock(Vector3 position, Vector3Int hitBlockWorldPosition, Block block)
    {
       playerBuildLink.PlaceBlock(position, hitBlockWorldPosition, block);
    }
}
