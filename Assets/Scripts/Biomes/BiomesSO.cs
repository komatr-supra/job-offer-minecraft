using UnityEngine;

[CreateAssetMenu(fileName = "BiomesSO", menuName = "job offer minecraft/BiomesSO", order = 0)]
public class BiomesSO : ScriptableObject {

    public Sprite background;
    public string biomeName;
    public float multiplier;
    public int softCubesHeightMax;
    public int minimalHeight;
    public int maximalHeight;
    public Block softBlock;
    public Block hardBlock;
    public Block topBlock;
}
