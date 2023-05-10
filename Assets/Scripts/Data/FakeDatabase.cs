using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDatabase : MonoBehaviour
{
    [SerializeField] private BiomesSO[] biomes;
    [SerializeField] private BlocksSO[] blocks;
    
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
    public BiomesSO GetBiome(Biome biome)
    {
        return biomes[(int)biome];
    }
    
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
        Snow
    }
    public enum Biome
    {
        Plains,
        Hills
    }
