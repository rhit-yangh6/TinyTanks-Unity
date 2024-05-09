using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [BurstCompile]
    public struct HorizontalMergeJob : IJobParallelFor
    {
        [ReadOnly] private readonly Vector2Int _density;
        private DisjointSet _disjointSet;

        public HorizontalMergeJob(DisjointSet disjointSet, Vector2Int density)
        {
            _disjointSet = disjointSet;
            _density = density;
        }

        [BurstCompile]
        public void Execute(int y)
        {
            for (int x = 0; x < _density.x - 1; ++x)
            {
                int index = y * _density.x + x;

                if (_disjointSet.IsFullQuad(index) && _disjointSet.IsFullQuad(index + 1))
                {
                    _disjointSet.Union(index, index + 1, Vector2Int.right);
                }
            }
        }
    }
}