using UnityEngine;

namespace TerraformingTerrain2d
{
    public readonly struct ChunkBounds
    {
        public readonly Vector2Int Start;
        public readonly Vector2Int End;

        public ChunkBounds(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;
        }

        public Vector2Int Size => End - Start;

        public override string ToString()
        {
            return $"Start = {Start} End = {End}";
        }
    }
}