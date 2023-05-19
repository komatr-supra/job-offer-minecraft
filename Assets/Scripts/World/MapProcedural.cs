using System;
using System.Collections.Generic;
using UnityEngine;

//procedural generator
namespace Map
{
    public class MapProcedural
    {
        private MapDataProvider mapDataProvider;
        private WorldConstructor worldConstructor;
        public MapProcedural(MapDataProvider mapDataProvider, WorldConstructor worldConstructor)
        {
            this.mapDataProvider = mapDataProvider;
            this.worldConstructor = worldConstructor;
        }
        private void CreateChunks(IEnumerable<Vector2Int> positions, Action onComplete = null)
        {
            foreach (Vector2Int position in positions)
            {
                mapDataProvider.GetChunk(position, out Chunk chunk);
                worldConstructor.SpawnChunk(chunk);                
            }
            onComplete?.Invoke();
        }
        private void RemoveChunks(IEnumerable<Vector2Int> positions)
        {
            foreach (Vector2Int position in positions)
            {
                if (mapDataProvider.GetChunk(position, out Chunk chunk))
                {
                    worldConstructor.DespawnChunk(chunk);
                    mapDataProvider.RemoveChunk(chunk);
                }
            }
        }
        public void StartMap(Vector2Int playerMapPosition, Action onComplete)
        {
            //generate chunks at player position     
            var pos = mapDataProvider.GetUsedMapPositions(playerMapPosition);
            CreateChunks(pos, onComplete);
        }
        public void PlayerMoved(Vector2Int previousPosition, Vector2Int newPosition)
        {
            Vector2Int offset = newPosition - previousPosition;
            int signX = Mathf.Sign(offset.x).ToInt();
            int signY = Mathf.Sign(offset.y).ToInt();

            int radius = mapDataProvider.VisionRange;
            for (int x = 0; x < Mathf.Abs(offset.x); x++)
            {
                int deltedX = previousPosition.x - (signX * radius);
                deltedX -= (x * signX);
                for (int yy = -radius + previousPosition.y; yy <= radius + previousPosition.y; yy++)
                {
                        Vector2Int[] pos = { new Vector2Int(deltedX, yy) };
                        RemoveChunks(pos);
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int deletedY = previousPosition.y - (signY * radius);
                deletedY -= (signX * y);
                for (int xx = -radius + previousPosition.x; xx <= radius + previousPosition.x; xx++)
                {
                        Vector2Int[] pos = { new Vector2Int(xx, deletedY) };
                        RemoveChunks(pos);
                }
            }
            for (int x = 0; x < Mathf.Abs(offset.x); x++)
            {
                int loadedX = newPosition.x + (signX * radius);
                loadedX += (x * signX);
                for (int yy = -radius + newPosition.y; yy <= radius + newPosition.y; yy++)
                {
                        Vector2Int[] mapPosition = { new Vector2Int(loadedX, yy) };
                        CreateChunks(mapPosition);
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int loadedY = newPosition.y + (signY * radius);
                loadedY += (y * signY);
                for (int xx = -radius + newPosition.x; xx <= radius + newPosition.x; xx++)
                {
                        Vector2Int[] pos = { new Vector2Int(xx, loadedY) };
                        CreateChunks(pos);
                }
            }
        }
    }
}
