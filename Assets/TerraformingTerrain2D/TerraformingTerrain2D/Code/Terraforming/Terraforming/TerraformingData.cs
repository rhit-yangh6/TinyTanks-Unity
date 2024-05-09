using UnityEngine;

namespace TerraformingTerrain2d
{
    public class TerraformingData
    {
        public readonly TerraformingTerrainCollider Collider;
        public readonly MarchingSquareInput Input;
        public readonly MeshFilter MeshFilter;
        public readonly ChunkBounds Bounds;
        public readonly GridData GridData;

        public TerraformingData(GridData gridData, ChunkBounds bounds, MarchingSquareInput input,
            TerraformingTerrainCollider collider, MeshFilter meshFilter)
        {
            MeshFilter = meshFilter;
            Collider = collider;
            GridData = gridData;
            Bounds = bounds;
            Input = input;
        }

        public Vector2Int ChunkSize => Bounds.Size;
        public int Iterations => ChunkSize.x * ChunkSize.y;
    }
}