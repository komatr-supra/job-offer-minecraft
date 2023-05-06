using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class MeshTest : MonoBehaviour
{
    private ProBuilderMesh meshMain;
    [SerializeField] private int width = 3;
    [SerializeField] private int height = 3;
    [SerializeField] private int depth = 3;
    [SerializeField] private readonly CubeTypeSO[] cubeType;
    private int numberOfSquares;
    private int numberOfFaces;
    private Dictionary<Vector3Int, int> walls;
    private Cell[] cells;
    
    Vector3Int[] verticesPos = {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };
    Vector3Int[] verticesFromCenterOffsets = {
        new Vector3Int(-1,-1,-1),//left bot front
        new Vector3Int(+1,-1,-1),//right bot front
        new Vector3Int(+1,-1,+1),//right bot rear
        new Vector3Int(-1,-1,+1),//left bot rear
        new Vector3Int(-1,+1,-1),//left top front
        new Vector3Int(+1,+1,-1),//right top front
        new Vector3Int(+1,+1,+1),//right top rear
        new Vector3Int(-1,+1,+1)//left top rear
    };
    Vector3Int[] centerOfWallOffsets = {
        new Vector3Int(+1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, +1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, +1),
        new Vector3Int(0, 0, -1)
    };
    [SerializeField] private Material mate;
    List<ProBuilderMesh> proBuilderMeshes;
    private void Start() {
        MakeGrid();
        ProBuilderMesh chunkMesh = ProBuilderMesh.Create();
        chunkMesh.Clear();
        var v = CombineMeshes.Combine(proBuilderMeshes, proBuilderMeshes[0]);
        foreach (var item in v)
        {
            Debug.Log(item);
            item.ToMesh();
            item.Refresh();            
        }
    }
    private void MakeGrid() {
        walls = new();
        numberOfSquares = width * height * depth;
        cells = new Cell[numberOfSquares];
        numberOfFaces = (width * height) + (width * height * depth) +
                        (height * depth) + (height * depth * width) +
                        (width * depth) + (width * depth * height);
        //Face[] wallFaces = new Face[numberOfFaces];
        //Vector3[] vertices = new Vector3[4 + 4 * numberOfSquares];
        proBuilderMeshes = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    int index1D = x + y * width + z * width * height;
                    //1 is for cube type index -> real data is shared for all cubes
                    //cells[index1D] = new Cell(1);
                    Vector3Int cellWorldPosition = new Vector3Int(x, y, z);
                        cellWorldPosition *= 2;
                    foreach (var item in centerOfWallOffsets)
                    {
                        Vector3Int wallPosition = item + cellWorldPosition;
                        
                        if(walls.ContainsKey(wallPosition)) continue;
                        //Debug.Log(wallPosition);
                        walls.Add(wallPosition, index1D);
                        Vector3[] wallVertices = GetWallQuadVertices(wallPosition, cellWorldPosition);
                        Face[] wallFace = { new Face(new int[] { 0, 1, 2, 0, 2, 3 })};

                        ProBuilderMesh quad = ProBuilderMesh.Create(wallVertices, wallFace);
                        quad.SetMaterial(quad.faces, mate);
                        quad.faces[0].submeshIndex = 0;
                        quad.ToMesh();
                        proBuilderMeshes.Add(quad);
                        //yield return new WaitForSeconds(1);
                    }
                }
            }
        }
        Debug.Log(proBuilderMeshes.Count);
        
    }
    

    private void CreateWallOfMesh()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                
            }
        }
    }
    private Vector3[] GetWallQuadVertices(Vector3Int wallCenter, Vector3Int cubePosition)
    {
        Vector3[] returnedVertices = new Vector3[4];
        //cubePosition *= 2;
        // X not the same (right left)
        if(wallCenter.x != cubePosition.x)
        {
            Debug.Log("right and left");
            returnedVertices[0] = new Vector3Int(0, -1, -1) + wallCenter;
            returnedVertices[1] = new Vector3Int(0, +1, -1) + wallCenter;
            returnedVertices[2] = new Vector3Int(0, +1, +1) + wallCenter;
            returnedVertices[3] = new Vector3Int(0, -1, +1) + wallCenter;
        }
        // Y is not the same (top bot)
        else if(wallCenter.y != cubePosition.y)
        {
            Debug.Log("top and bot");
            returnedVertices[0] = new Vector3Int(-1, 0, -1) + wallCenter;
            returnedVertices[1] = new Vector3Int(-1, 0, +1) + wallCenter;
            returnedVertices[2] = new Vector3Int(+1, 0, +1) + wallCenter;
            returnedVertices[3] = new Vector3Int(+1, 0, -1) + wallCenter;
        }
        else
        {
            Debug.Log("front and rear");
            returnedVertices[0] = new Vector3Int(-1, -1, 0) + wallCenter;
            returnedVertices[1] = new Vector3Int(-1, +1, 0) + wallCenter;
            returnedVertices[2] = new Vector3Int(+1, +1, 0) + wallCenter;
            returnedVertices[3] = new Vector3Int(+1, -1, 0) + wallCenter;
        }
        return returnedVertices;
    }
}
public struct Cell
{
    public byte cubeTypeID;
    public Cell(byte cubeTypeID)
    {
        this.cubeTypeID = cubeTypeID;
    }
}

