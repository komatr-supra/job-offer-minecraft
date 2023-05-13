using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        private void CreateChunks(IEnumerable<Vector2Int> positions)
        {
            foreach (Vector2Int position in positions)
            {
                if(!mapDataProvider.GetChunk(position, out Chunk chunk))
                {
                    worldConstructor.SpawnChunk(chunk);
                    mapDataProvider.AddChunk(chunk);
                }
            }
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
        public void StartMap(Vector2Int playerMapPosition)
        {
            //generate chunks at player position     
            var pos = mapDataProvider.GetUsedMapPositions(playerMapPosition);
            CreateChunks(pos);
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
                for (int yy = -radius; yy <= radius; yy++)
                {
                    if(mapDataProvider.GetChunk(new Vector2Int(deltedX, yy), out Chunk chunk))
                    {
                        Vector2Int[] pos = { new Vector2Int(deltedX, yy) };
                        RemoveChunks(pos);
                    }
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int deletedY = previousPosition.y - (signY * radius);
                deletedY -= (signX * y);
                for (int xx = -radius; xx <= radius; xx++)
                {
                    if(mapDataProvider.GetChunk(new Vector2Int(xx, deletedY), out Chunk chunk))
                    {
                        Vector2Int[] pos = { new Vector2Int(xx, deletedY) };
                        RemoveChunks(pos);
                    }
                }
            }
            for (int x = 0; x < Mathf.Abs(offset.x); x++)
            {
                int loadedX = previousPosition.x + (signX * radius);
                loadedX += (x * signX);
                for (int yy = -radius; yy <= radius; yy++)
                {
                    if(!mapDataProvider.GetChunk(new Vector2Int(loadedX, yy), out Chunk chunk))
                    {
                        Vector2Int[] mapPosition = { new Vector2Int(loadedX, yy) };
                        CreateChunks(mapPosition);
                    }
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int loadedY = previousPosition.y + (signY * radius);
                loadedY += (y * signY);
                for (int xx = -radius; xx <= radius; xx++)
                {
                    if(mapDataProvider.GetChunk(new Vector2Int(xx, loadedY), out Chunk chunk))
                    {
                        Vector2Int[] pos = { new Vector2Int(xx, loadedY) };
                        CreateChunks(pos);
                    }
                }
            }
        }
    }
}
