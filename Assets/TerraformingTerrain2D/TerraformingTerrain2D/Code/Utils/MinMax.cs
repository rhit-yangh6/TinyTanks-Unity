using UnityEngine;

namespace TerraformingTerrain2d
{
    public readonly struct MinMax
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public Vector4 Value => new(Min.x, Min.y, Max.x, Max.y);

        public MinMax(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }
    }
}