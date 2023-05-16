using System.Collections.Generic;
using UnityEngine;

//this is simple map generator, in real game it will be much longer
//maybe keep track of biomes?
namespace Map
{
    public class MapGenerator
    {
        //this is basicly world save data
        //solid blocks (stone)
        private Vector2 solidOffset;
        //soft blocks (dirt, grass, snow)
        private Vector2 softOffset;
        public MapGenerator()
        {
            solidOffset.x = UnityEngine.Random.Range(0f, 1000000f);
            solidOffset.y = UnityEngine.Random.Range(0f, 1000000f);
            softOffset.x = UnityEngine.Random.Range(0f, 1000000f);
            softOffset.y = UnityEngine.Random.Range(0f, 1000000f);
        }
        //get data based on offset, with same offset(same seed) will be map same
        public IEnumerable<MapData> GetMapDatas(IEnumerable<Vector2Int> position)
        {
            foreach (var item in position)
            {
                var biom = FakeDatabase.Instance.selectedBiome;                         //TEST
                yield return new MapData(biom, item, softOffset, solidOffset);
            }
        }
    }
}
