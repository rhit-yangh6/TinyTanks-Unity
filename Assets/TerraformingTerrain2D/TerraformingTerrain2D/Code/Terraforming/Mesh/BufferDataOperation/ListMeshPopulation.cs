using System.Collections.Generic;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ListMeshPopulation<T> : MeshPopulation where T : struct
    {
        private readonly List<T> _vertices;
        private readonly List<int> _triangles;

        public ListMeshPopulation(MeshFilter meshFilter, List<T> vertices, List<int> triangles) : base(meshFilter)
        {
            _vertices = vertices;
            _triangles = triangles;
        }

        protected override int VerticesCount => _vertices.Count;
        protected override int TrianglesCount => _triangles.Count;

        protected override void SetVertexBuffer()
        {
            MeshFilter.sharedMesh.SetVertexBufferData(_vertices, 0, 0, VerticesCount);
        }

        protected override void SetIndexBuffer()
        {
            MeshFilter.sharedMesh.SetIndexBufferData(_triangles, 0, 0, TrianglesCount);
        }
    }
}