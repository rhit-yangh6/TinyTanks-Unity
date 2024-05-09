using UnityEngine;

namespace TerraformingTerrain2d
{
    public readonly ref struct SquareIndex
    {
        public readonly Vector2Int ChunkIndex;
        public readonly Vector2Int GlobalIndex;

        public SquareIndex(int oneDimensionalIndex, ChunkBounds chunkBounds)
        {
            ChunkIndex = new Vector2Int()
            {
                x = oneDimensionalIndex % chunkBounds.Size.x,
                y = oneDimensionalIndex / chunkBounds.Size.x,
            };

            GlobalIndex = new Vector2Int()
            {
                x = ChunkIndex.x + chunkBounds.Start.x,
                y = ChunkIndex.y + chunkBounds.Start.y,
            };
        }

        public int ChunkX => ChunkIndex.x;
        public int ChunkY => ChunkIndex.y;

        public int GlobalX => GlobalIndex.x;
        public int GlobalY => GlobalIndex.y;
    }
}