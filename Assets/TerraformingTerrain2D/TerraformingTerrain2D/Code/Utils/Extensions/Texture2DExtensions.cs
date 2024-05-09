using UnityEngine;

namespace TerraformingTerrain2d
{
    public static class Texture2DExtensions
    {
        public static float GetRatio(this Texture2D texture2D)
        {
            return (float)texture2D.width / texture2D.height;
        }

        public static Vector2Int GetResolution(this Texture2D texture2D)
        {
            return new Vector2Int(texture2D.width, texture2D.height);
        }
    }
}