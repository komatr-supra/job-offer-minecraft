using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDatabase : MonoBehaviour
{
    [SerializeField] private BiomesSO[] biomes;
    [SerializeField] private BlocksSO[] blocks;
    //just for test use!!! public access
    public BiomesSO selectedBiome;

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
}
public enum Block
    {
        //its for human use only -> its indexes of blockData
        //trying make minimal cube data
        none,
        Dirt,
        Stone,
        Snow,
        Grass,
        Sand,
        MountainStone,
        Grass2
    }
    public enum Biome
    {
        Plains,
        Hills
    }
