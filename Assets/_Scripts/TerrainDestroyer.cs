using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts
{
    public class TerrainDestroyer : MonoBehaviour
    {

        public Tilemap terrain;
        public static TerrainDestroyer Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            }
            else
            {
                Instance = this;
            }
        }

        public void DestroyTerrainCircular(Vector3 explosionLocation, float radius)
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
                    if (terrain.GetTile(tilePos) != null && dx2PlusDy2 <= radiusSquared)
                    {
                        DestroyTile(tilePos);
                    }
                }
            }
        }
        
        public void DestroyTerrainSquare(Vector3 explosionLocation, float radius)
        {
            for (float x = -radius; x < radius; x += 0.64f)
            {
                for (float y = -radius; y < radius; y += 0.64f)
                {
                    Vector3Int tilePos = terrain.WorldToCell(explosionLocation + new Vector3(x, y, 0));
                    DestroyTile(tilePos);
                }
            }
        }

        public void DestroyTile(Vector3Int tilePos)
        {
            terrain.SetTile(tilePos, null);
        }
    }
}
