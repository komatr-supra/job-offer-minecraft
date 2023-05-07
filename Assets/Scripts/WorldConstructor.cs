using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldConstructor
{
    public GameObject CreateBlock(Vector3Int worldPosition, Block block)
    {
        BlocksSO blockData = FakeDatabase.Instance.GetBlock(block);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //block is empty dumb object
        cube.transform.position = worldPosition;
        cube.layer = 3;
        cube.GetComponent<MeshRenderer>().material = blockData.material;
        return cube;
    }
}
