using System.Collections.Generic;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class SingleThreadedMeshPopulation
    {
        private readonly ListMeshPopulation<TerrainVertex> _population;
        private readonly VertexCaching _vertexCaching = new();
        private readonly List<int> _triangles = new();
        private readonly Vector2Int _squareDensity;

        public SingleThreadedMeshPopulation(MeshFilter meshFilter, Vector2Int squareDensity)
        {
            _population = new ListMeshPopulation<TerrainVertex>(meshFilter, _vertexCaching.Vertices, _triangles);
            _squareDensity = squareDensity;
        }

        public void Clear()
        {
            _vertexCaching.Clear();
            _triangles.Clear();
        }

        public void BufferData()
        {
            _population.BufferData();
        }

        public void AddMarchingSquare(MarchingSquareMeshConfiguration configuration)
        {
            TryAddVertices(configuration.Vertices, configuration.UV);
            AddTrisToMesh(configuration.Vertices, configuration.Triangles);
        }

        private void TryAddVertices(Vector2[] squareVertices, Vector2[] uv)
        {
            for (int i = 0; i < squareVertices.Length; ++i)
            {
                _vertexCaching.TryAdd(squareVertices[i], uv[i]);
            }
        }

        private void AddTrisToMesh(Vector2[] vertices, int[] triangles)
        {
            for (int i = 0; i < triangles.Length; ++i)
            {
                int localIndex = triangles[i];
                Vector2 indexedVertex = vertices[localIndex];
                TerrainVertex vertex = new(indexedVertex, Vector2.zero);
                int indexToCachedVertex = _vertexCaching.GetIndex(vertex);

                _triangles.Add(indexToCachedVertex);
            }
        }

        public void AddRectangle(RectangularDecompositionResult result, Vector2Int gridMin)
        {
            DecomposedRectangleCalculator calculator = new(_squareDensity, gridMin, stackalloc TerrainVertex[6]);
            calculator.Evaluate(result);

            for (int i = 0; i < 6; ++i)
            {
                _vertexCaching.TryAdd(calculator.Triangles[i]);
                _triangles.Add(_vertexCaching.GetIndex(calculator.Triangles[i]));
            }
        }
    }
}