using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace TerraformingTerrain2d
{
    [BurstCompile]
    public struct DistinctVerticesJob : IJobParallelFor
    {
        [ReadOnly] private NativeArray<TerrainVertex> _triangles;
        [WriteOnly] private NativeParallelHashSet<TerrainVertex>.ParallelWriter _distinctVerticesWriter;

        public DistinctVerticesJob(NativeArray<TerrainVertex> triangles, NativeParallelHashSet<TerrainVertex>.ParallelWriter distinctVerticesWriter)
        {
            _triangles = triangles;
            _distinctVerticesWriter = distinctVerticesWriter;
        }

        [BurstCompile]
        public void Execute(int index)
        {
            _distinctVerticesWriter.Add(_triangles[index]);
        }
    }
}