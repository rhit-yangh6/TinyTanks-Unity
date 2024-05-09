using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace TerraformingTerrain2d
{
    [BurstCompile]
    public struct PopulateIndicesJob : IJobParallelFor
    {
        [ReadOnly] private NativeArray<TerrainVertex> _vertices;
        [WriteOnly] private NativeParallelHashMap<TerrainVertex, int>.ParallelWriter _indices;

        public PopulateIndicesJob(NativeArray<TerrainVertex> vertices, NativeParallelHashMap<TerrainVertex, int>.ParallelWriter indices)
        {
            _vertices = vertices;
            _indices = indices;
        }

        [BurstCompile]
        public void Execute(int index)
        {
            _indices.TryAdd(_vertices[index], index);
        }
    }
}