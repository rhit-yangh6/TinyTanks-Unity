using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = _Scripts.Tiles.TileData;

namespace _Scripts
{
    public class TerrainDestroyer : MonoBehaviour
    {
        public Tilemap terrain;
        public static TerrainDestroyer instance;
        
        [SerializeField]
        private List<TileData> tileDataList;

        private Dictionary<TileBase, TileData> dataFromTiles;

        private void Awake()
        {
            dataFromTiles = new Dictionary<TileBase, TileData>();
            foreach (var tileData in tileDataList)
            {
                foreach (var tile in tileData.tiles)
                {
                    dataFromTiles.Add(tile, tileData);
                }
            }
            
            if (instance != null && instance != this) 
            { 
                Destroy(this); 
            }
            else
            {
                instance = this;
            }
        }

        public void DestroyTerrainCircular(Vector3 explosionLocation, float radius, int destroyingPower = 1)
        {
            /*
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionLocation, 200, layerMask);
            Debug.Log(hitColliders.Length);
            
            foreach(Collider2D col in hitColliders)
            {
                Debug.Log(col.gameObject);
            }
        
            BoundsInt bounds = terrain.cellBounds;
            TileBase[] allTiles = terrain.GetTilesBlock(bounds);

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    
                    if (tile != null)
                    {
                        // you may not need to find gridPlace, in which case cut next line
                        Vector3Int gridPlace = new Vector3Int(
                            x + bounds.xMin, y + bounds.yMin, bounds.z);

                        // do something
                    }
                }
            }
            
             */
            
            var radiusSquared = radius * radius;
            
            for (float x = -radius; x < radius; x += 0.64f)
            {
                for (float y = -radius; y < radius; y += 0.64f)
                {
                    Vector3Int tilePos = terrain.WorldToCell(explosionLocation + new Vector3(x, y, 0));
                    float dx2PlusDy2 = x * x + y * y;
                    TileBase currentTile = terrain.GetTile(tilePos);
                    if (currentTile != null && dx2PlusDy2 <= radiusSquared 
                                            && destroyingPower >= dataFromTiles[currentTile].hardness)
                    {
                        DestroyTile(tilePos);
                    }
                }
            }
        }
        
        public void DestroyTerrainSquare(Vector3 explosionLocation, float radius, int destroyingPower = 1)
        {
            for (float x = -radius; x < radius; x += 0.64f)
            {
                for (float y = -radius; y < radius; y += 0.64f)
                {
                    Vector3Int tilePos = terrain.WorldToCell(explosionLocation + new Vector3(x, y, 0));
                    TileBase currentTile = terrain.GetTile(tilePos);
                    if (currentTile != null && destroyingPower >= dataFromTiles[currentTile].hardness)
                    {
                        DestroyTile(tilePos);
                    }
                }
            }
        }

        private void DestroyTile(Vector3Int tilePos)
        {
            terrain.SetTile(tilePos, null);
        }
    }
}
