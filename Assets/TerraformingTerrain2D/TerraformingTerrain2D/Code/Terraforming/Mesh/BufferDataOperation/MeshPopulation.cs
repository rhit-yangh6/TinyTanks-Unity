using UnityEngine;
using UnityEngine.Rendering;

namespace TerraformingTerrain2d
{
    public abstract class MeshPopulation
    {
        protected readonly MeshFilter MeshFilter;

        private readonly VertexAttributeDescriptor[] _layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
        };

        protected MeshPopulation(MeshFilter meshFilter)
        {
            MeshFilter = meshFilter;
        }

        protected abstract int VerticesCount { get; }
        protected abstract int TrianglesCount { get; }

        public void BufferData()
        {
            MeshFilter.sharedMesh.SetVertexBufferParams(VerticesCount, _layout);
            SetVertexBuffer();

            MeshFilter.sharedMesh.SetIndexBufferParams(TrianglesCount, IndexFormat.UInt32);
            SetIndexBuffer();

            MeshFilter.sharedMesh.subMeshCount = 1;
            MeshFilter.sharedMesh.SetSubMesh(0, new SubMeshDescriptor(0, TrianglesCount),
                MeshUpdateFlags.DontRecalculateBounds);
        }

        protected abstract void SetVertexBuffer();
        protected abstract void SetIndexBuffer();
    }
}