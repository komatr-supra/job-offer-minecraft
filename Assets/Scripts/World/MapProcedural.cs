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
                mapDataProvider.GetChunk(position, out Chunk chunk);

                worldConstructor.SpawnChunk(chunk);
                
            }
        }
        private void RemoveChunks(IEnumerable<Vector2Int> positions)
        {
            foreach (Vector2Int position in positions)
            {
                //Debug.Log("want remove chunk " + position);
                if (mapDataProvider.GetChunk(position, out Chunk chunk))
                {
                    //Debug.Log("chunk exist and will be despawned");
                    worldConstructor.DespawnChunk(chunk);
                    mapDataProvider.RemoveChunk(chunk);
                }
                //else Debug.Log("chunk NOT exist!!!");
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
            //Debug.Log("player moved from " + previousPosition.x + "x; " + previousPosition.y + "y\nto position " + newPosition.x + "x; " + newPosition.y + "y" );
            int signX = Mathf.Sign(offset.x).ToInt();
            int signY = Mathf.Sign(offset.y).ToInt();

            int radius = mapDataProvider.VisionRange;
            for (int x = 0; x < Mathf.Abs(offset.x); x++)
            {
                int deltedX = previousPosition.x - (signX * radius);
                deltedX -= (x * signX);
                for (int yy = -radius + previousPosition.y; yy <= radius + previousPosition.y; yy++)
                {
                    //Debug.Log(deltedX + " del " + yy);
                    
                        Vector2Int[] pos = { new Vector2Int(deltedX, yy) };
                        RemoveChunks(pos);
                        /*
                    if(mapDataProvider.GetChunk(new Vector2Int(deltedX, yy), out Chunk chunk))
                    {
                    }
                    */
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int deletedY = previousPosition.y - (signY * radius);
                deletedY -= (signX * y);
                for (int xx = -radius + previousPosition.x; xx <= radius + previousPosition.x; xx++)
                {
                    //Debug.Log(xx + " del " + deletedY);
                    
                        Vector2Int[] pos = { new Vector2Int(xx, deletedY) };
                        RemoveChunks(pos);
                        /*
                    if(mapDataProvider.GetChunk(new Vector2Int(xx, deletedY), out Chunk chunk))
                    {
                    }
                    */
                }
            }
            for (int x = 0; x < Mathf.Abs(offset.x); x++)
            {
                int loadedX = newPosition.x + (signX * radius);
                loadedX += (x * signX);
                for (int yy = -radius + newPosition.y; yy <= radius + newPosition.y; yy++)
                {
                    //Debug.Log(loadedX + " load " + yy);
                    
                        Vector2Int[] mapPosition = { new Vector2Int(loadedX, yy) };
                        CreateChunks(mapPosition);/*
                    if(!mapDataProvider.GetChunk(new Vector2Int(loadedX, yy), out Chunk chunk))
                    {
                    }
                    */
                }
            }
            for (int y = 0; y < Mathf.Abs(offset.y); y++)
            {
                int loadedY = newPosition.y + (signY * radius);
                loadedY += (y * signY);
                for (int xx = -radius + newPosition.x; xx <= radius + newPosition.x; xx++)
                {
                    //Debug.Log(xx + " load " + loadedY);
                    
                        Vector2Int[] pos = { new Vector2Int(xx, loadedY) };
                        CreateChunks(pos);/*
                    if(mapDataProvider.GetChunk(new Vector2Int(xx, loadedY), out Chunk chunk))
                    {
                    }
                    */
                }
            }
        }
    }
}
