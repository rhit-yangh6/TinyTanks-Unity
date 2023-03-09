using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Tiles
{
    [CreateAssetMenu]
    public class TileData : ScriptableObject
    {
        public TileBase[] tiles;

        public int hardness;
    }
}
