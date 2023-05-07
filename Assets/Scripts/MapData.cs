using UnityEngine;

public struct MapData
{
    public BiomesSO biome;
    public Vector2Int position;
    public Vector2 softOffset;
    public Vector2 hardOffset;
    public MapData(BiomesSO biome, Vector2Int position, Vector2 softOffset, Vector2 hardOffset)
    {
        this.biome = biome;
        this.position = position;
        this.softOffset = softOffset;
        this.hardOffset = hardOffset;
    }
}
