using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ChunkBoundsCalculator
    {
        public readonly Vector2Int GeneratedChunksCount;
        private readonly Vector2Int _scalarFieldSize;

        public ChunkBoundsCalculator(Vector2Int generatedChunksCount, Vector2Int scalarFieldSize)
        {
            GeneratedChunksCount = generatedChunksCount;
            _scalarFieldSize = scalarFieldSize;
        }

        public ChunkBounds Evaluate(int x, int y)
        {
            return new ChunkBounds(EvaluateTarget(x, y), EvaluateTarget(x + 1, y + 1));
        }

        private Vector2Int EvaluateTarget(int x, int y)
        {
            Vector2Int chunkSize = Vector2Int.FloorToInt((Vector2)_scalarFieldSize / GeneratedChunksCount);

            Vector2Int result = new();
            result.x = x * chunkSize.x;
            result.y = y * chunkSize.y;

            if (x == GeneratedChunksCount.x)
                result.x = _scalarFieldSize.x - 1;

            if (y == GeneratedChunksCount.y)
                result.y = _scalarFieldSize.y - 1;

            return result;
        }
    }
}