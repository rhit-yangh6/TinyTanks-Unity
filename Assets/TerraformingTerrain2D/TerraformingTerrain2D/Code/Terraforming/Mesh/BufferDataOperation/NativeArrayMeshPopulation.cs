using Unity.Collections;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class NativeArrayMeshPopulation<T> : MeshPopulation where T : struct
    {
        private NativeArray<T> _vertices;
        private NativeArray<int> _triangles;

        public NativeArrayMeshPopulation(MeshFilter meshFilter) : base(meshFilter)
        {
        }

        protected override int VerticesCount => _vertices.Length;
        protected override int TrianglesCount => _triangles.Length;

        public void SetMeshData(NativeArray<T> vertices, NativeArray<int> triangles)
        {
            _vertices = vertices;
            _triangles = triangles;
        }

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