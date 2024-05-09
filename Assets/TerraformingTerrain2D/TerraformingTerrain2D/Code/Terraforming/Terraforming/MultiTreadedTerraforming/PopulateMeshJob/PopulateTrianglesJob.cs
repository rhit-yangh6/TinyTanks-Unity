using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace TerraformingTerrain2d
{
    [BurstCompile]
    public struct PopulateTrianglesJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] [WriteOnly]
        private NativeArray<int> _indices;

        [ReadOnly] private NativeParallelHashMap<TerrainVertex, int> _indicesHashMap;
        [ReadOnly] private NativeArray<TerrainVertex> _triangles;

        public PopulateTrianglesJob(NativeArray<TerrainVertex> triangles, NativeParallelHashMap<TerrainVertex, int> indicesHashMap, NativeArray<int> indices)
        {
            _indicesHashMap = indicesHashMap;
            _triangles = triangles;
            _indices = indices;
        }

        [BurstCompile]
        public void Execute(int index)
        {
            SetIndex(index, 0);
            SetIndex(index, 1);
            SetIndex(index, 2);
        }

        private void SetIndex(int triangleIndex, int offset)
        {
            int triangleSubIndex = triangleIndex * 3 + offset;
            TerrainVertex vertex = _triangles[triangleSubIndex];
            int vertexIndex = _indicesHashMap[vertex];

            _indices[triangleSubIndex] = vertexIndex;
        }
    }
}