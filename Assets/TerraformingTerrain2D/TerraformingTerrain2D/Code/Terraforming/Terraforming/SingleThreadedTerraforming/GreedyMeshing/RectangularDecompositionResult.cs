using UnityEngine;

namespace TerraformingTerrain2d
{
    public readonly ref struct RectangularDecompositionResult
    {
        public readonly Vector2Int MaxGridIndex;
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public RectangularDecompositionResult(Vector2Int maxGridIndex, Vector2 min, Vector2 max)
        {
            MaxGridIndex = maxGridIndex;
            Min = min;
            Max = max;
        }
    }
}