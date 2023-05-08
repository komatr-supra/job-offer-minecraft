using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool
{
    private const int prewarmCubesAmount = 100000;
    private Queue<GameObject> unusedCubes;
    private Dictionary<Vector3Int, GameObject> usedCubes;
    public BlockPool()
    {
        usedCubes = new Dictionary<Vector3Int, GameObject>();
        unusedCubes = new Queue<GameObject>();

        for (int i = 0; i < prewarmCubesAmount; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.layer = 3;
            cube.SetActive(false);
            cube.transform.SetParent(FakeDatabase.Instance.transform);
            unusedCubes.Enqueue(cube);
        }
    }
    public void SetCube(Vector3Int worldPosition, Block block)
    {
        if(usedCubes.ContainsKey(worldPosition)) return;
        var cube = unusedCubes.Dequeue();
        cube.transform.position = worldPosition;
        BlocksSO blockData = FakeDatabase.Instance.GetBlock(block);
        cube.GetComponent<MeshRenderer>().material = blockData.material;
        cube.SetActive(true);
        usedCubes.Add(worldPosition, cube);
    }
    public void DisableCube(Vector3Int worldPosition)
    {
        if(!usedCubes.ContainsKey(worldPosition)) return;
        var cube = usedCubes[worldPosition];
        cube.SetActive(false);
        usedCubes.Remove(worldPosition);
        unusedCubes.Enqueue(cube);
    }
}
