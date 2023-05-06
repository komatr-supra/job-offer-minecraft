using UnityEngine;

[CreateAssetMenu(fileName = "BiomesSO", menuName = "job offer minecraft/BiomesSO", order = 0)]
public class BiomesSO : ScriptableObject {

    public string biomeName;
    public float multiplier;
    public int softCubesHeightMax;
    public BlocksSO softBlock;
    public BlocksSO hardBlock;
    public BlocksSO topBlock;
}
