using System;
using Unity.Collections;

namespace TerraformingTerrain2d
{
    public struct NativeMarchingSquareMeshConfiguration : IDisposable
    {
        [NativeDisableParallelForRestriction] public NativeArray<int> Triangles;
        public readonly int VerticesCount;

        public NativeMarchingSquareMeshConfiguration(int verticesCount, int[] triangles)
        {
            Triangles = new NativeArray<int>(triangles, Allocator.Persistent);
            VerticesCount = verticesCount;
        }

        public void Dispose()
        {
            Triangles.Dispose();
        }
    }
}