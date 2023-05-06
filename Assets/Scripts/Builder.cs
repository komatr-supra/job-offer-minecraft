using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using TMPro;
public class Builder : MonoBehaviour
{
    [SerializeField] private Transform lookTransform;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float sphereCastRadius = 0.01f;
    [SerializeField] private float buildingDistance = 3f;
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private LayerMask layerMaskGround;

    [SerializeField] private ChunkGenerator mapGenerator;
    [SerializeField] private float timeToBreakCube = 2f;
    [SerializeField] private TextMeshProUGUI tmpro;
    private bool isBuildingMode;
    private bool onMainAction;
    private bool onSecondaryAction;
    private GameObject visual;
    private Coroutine digCoroutine;
    private void Start()
    {
        visual = Instantiate(visualPrefab);
        visual.SetActive(false);
        isBuildingMode = true;
        StartCoroutine(StartBuildCoroutine());
    }
    public void OnMainAction(InputValue inputValue)
    {
        MakeRaycast(out var raycast);
        var vv = raycast.collider.transform.position.ToVec3Int();
        //var v = mapGenerator.GetWAllsID(vv);
        string str = "";
        //foreach (var item in v)
        //{
        //    str += item + "\n";
        //}
        tmpro.text = str;
        Debug.Log("main" + inputValue.isPressed);
        onMainAction = inputValue.isPressed;
        if(onMainAction) digCoroutine = StartCoroutine(DigCoroutine());
        
    }

    private IEnumerator DigCoroutine()
    {
        if(!MakeRaycast(out var raycastHit)) yield break;
        Vector3Int cubePositionToDestroy = raycastHit.collider.transform.position.ToVec3Int();
        float counter = 0;
        while(MakeRaycast(out var raycast) && onMainAction && !onSecondaryAction)
        {
            if(cubePositionToDestroy != raycast.collider.transform.position.ToVec3Int()) yield break;
            counter += Time.deltaTime;
            yield return null;
            if(counter > timeToBreakCube)
            {
                BreakBlock(cubePositionToDestroy);
                yield break;
            }
        }
        
    }

    private void BreakBlock(Vector3Int cubePosition)
    {
        //mapGenerator.BreakBlock(cubePosition);
    }

    public void OnSecondaryAction(InputValue inputValue)
    {
        Debug.Log("sec" + inputValue.isPressed);
        onSecondaryAction = inputValue.isPressed;
    }
    public void OnBuild(InputValue inputValue)
    {
        if(inputValue.isPressed == false) return;
        isBuildingMode = !isBuildingMode;
        if(isBuildingMode)
        {
            StartCoroutine(StartBuildCoroutine());
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
                List<Vector3> neighbourFreePosition = mapGenerator.GetFreeNeighbourPosition(raycastHit.collider.transform.position.ToVec3Int());
                Vector3 visualPosition = neighbourFreePosition.OrderBy(vector => Vector3.Distance(pointToCompare, vector)).First();
                visual.transform.position = visualPosition;
                visual.SetActive(true);
                if (onSecondaryAction)
                {
                    PlaceBlock(visualPosition);
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

    private void PlaceBlock(Vector3 position)
    {
        mapGenerator.PlaceBlock(position);
    }
}
