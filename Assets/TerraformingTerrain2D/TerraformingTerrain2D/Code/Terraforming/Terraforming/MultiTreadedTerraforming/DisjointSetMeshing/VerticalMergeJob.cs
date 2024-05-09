using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [BurstCompile]
    public struct VerticalMergeJob : IJobParallelFor
    {
        [ReadOnly] private readonly Vector2Int _density;
        private DisjointSet _disjointSet;

        public VerticalMergeJob(DisjointSet disjointSet, Vector2Int density)
        {
            _disjointSet = disjointSet;
            _density = density;
        }

        [BurstCompile]
        public void Execute(int x)
        {
            for (int y = 0; y < _density.y - 1; ++y)
            {
                int index = y * _density.x + x;

                if (SetsAreSuitableForMerging(index, index + _density.x))
                {
                    _disjointSet.Union(index, index + _density.x, Vector2Int.up);
                }
            }
        }

        private bool SetsAreSuitableForMerging(int bottom, int top)
        {
            if (_disjointSet.IsFullQuad(bottom) && _disjointSet.IsFullQuad(top))
            {
                if (_disjointSet.IsRoot(top))
                {
                    int firstRowX = _disjointSet.GetRootIndex(bottom) % _density.x;
                    int secondRowX = _disjointSet.GetRootIndex(top) % _density.x;

                    return firstRowX == secondRowX && // at same position with same length 
                           _disjointSet.GetSize(bottom).x == _disjointSet.GetSize(top).x;
                }
            }

            return false;
        }
    }
}