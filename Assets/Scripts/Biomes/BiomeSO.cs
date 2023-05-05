using UnityEngine;

[CreateAssetMenu(fileName = "NewBiomeData", menuName = "Bime Data/Create Biome Data", order = 0)]
public class BiomeSO : ScriptableObject {
    public string terrainName;
    public float multiplier;
    public int softCubesHeightMax;

}
