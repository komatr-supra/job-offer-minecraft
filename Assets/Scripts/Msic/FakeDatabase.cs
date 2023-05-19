using UnityEngine;

//this is database, used for save and chunk data(int can be saved, and this is ushort -> small)
public class FakeDatabase : MonoBehaviour
{
    [SerializeField] private BiomesSO[] biomes;
    [SerializeField] private BlocksSO[] blocks;
    //just for test use!!! public access
    public BiomesSO selectedBiome;
    [SerializeField] private GameObject dropPrefab;
    public GameObject DropPrefab => dropPrefab;
    //dont know if it is a good way all block is here, must track it, but reference is only int(ushort)
    public static FakeDatabase Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public BiomesSO[] GetBiomes() => biomes;
    public BlocksSO GetBlock(Block blockEnum)
    {
        return blocks[(int)blockEnum];
    }
    public int GetBlocksSOIndex(BlocksSO block)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if(blocks[i] == block) return i;
        }
        return -1;
    }
}
public enum Block
    {
        //its for human use only -> its indexes of blockData
        //trying make minimal cube data... its not best...
        none,
        Dirt,
        Stone,
        Snow,
        Grass,
        Sand,
        MountainStone,
        Grass2
    }
