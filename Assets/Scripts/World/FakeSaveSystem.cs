using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSaveSystem : MonoBehaviour
{
    public static FakeSaveSystem Instance;
    private Dictionary<Vector2Int, uint[]> saves;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            saves = new();
            return;
        }
        Destroy(gameObject);
    }
    public bool TryGetSave(Vector2Int chunkPos, out uint[] save)
    {
        if(saves.TryGetValue(chunkPos, out save)) return true;
        return false;
    }
    public bool SaveData(Vector2Int chunkPos, uint[] save)
    {
        return saves.TryAdd(chunkPos, save);
    }
}
