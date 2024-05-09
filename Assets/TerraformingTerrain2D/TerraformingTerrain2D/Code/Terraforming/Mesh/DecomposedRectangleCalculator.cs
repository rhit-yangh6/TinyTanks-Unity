using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public readonly ref struct DecomposedRectangleCalculator
    {
        public readonly Span<TerrainVertex> Triangles;
        private readonly Vector2Int _gridMin;
        private readonly Vector2Int _density;

        public DecomposedRectangleCalculator(Vector2Int density, Vector2Int gridMin, Span<TerrainVertex> triangles)
        {
            Triangles = triangles;
            _gridMin = gridMin;
            _density = density;
        }

        public void Evaluate(RectangularDecompositionResult result)
        {
            TerrainVertex bottomLeft = new(new Vector3(result.Min.x, result.Min.y),
                new Vector2(_gridMin.x, _gridMin.y) / _density);
            TerrainVertex bottomRight = new(new Vector3(result.Max.x, result.Min.y),
                new Vector2(result.MaxGridIndex.x, _gridMin.y) / _density);
            TerrainVertex topLeft = new(new Vector3(result.Min.x, result.Max.y),
                new Vector2(_gridMin.x, result.MaxGridIndex.y) / _density);
            TerrainVertex topRight = new(new Vector3(result.Max.x, result.Max.y),
                new Vector2(result.MaxGridIndex.x, result.MaxGridIndex.y) / _density);

            Triangles[0] = bottomLeft;
            Triangles[1] = topLeft;
            Triangles[2] = bottomRight;

            Triangles[3] = topLeft;
            Triangles[4] = topRight;
            Triangles[5] = bottomRight;
        }

        public void GetVertices(ref Span<Vector2> vertices)
        {
            vertices[0] = Triangles[0].Position;
            vertices[1] = Triangles[1].Position;
            vertices[2] = Triangles[2].Position;
            vertices[3] = Triangles[4].Position;
        }
    }
}