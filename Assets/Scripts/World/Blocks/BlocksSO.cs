using UnityEngine;

[CreateAssetMenu(fileName = "BlocksSO", menuName = "job offer minecraft/BlocksSO", order = 0)]
public class BlocksSO : ScriptableObject {
    public string blockName;
    public Material material;
    public float minigTime;
    public bool isReplaceable;
    public Sprite invSprite;
}
