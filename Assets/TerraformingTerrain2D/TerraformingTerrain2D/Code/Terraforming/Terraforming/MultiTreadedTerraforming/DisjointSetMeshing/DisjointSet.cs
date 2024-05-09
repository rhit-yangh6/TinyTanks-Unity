using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public struct DisjointSet : IDisposable
    {
        [NativeDisableContainerSafetyRestriction]
        private NativeArray<int> _value;

        [NativeDisableContainerSafetyRestriction]
        private NativeArray<Vector2Int> _sizes;

        public DisjointSet(int chunkSize, Allocator allocator)
        {
            _value = new NativeArray<int>(chunkSize, allocator);
            _sizes = new NativeArray<Vector2Int>(chunkSize, allocator);
        }

        public bool IsFullQuad(int index) => _value[index] != int.MaxValue;
        public readonly bool IsRoot(int index) => _value[index] < 0;
        public readonly Vector2Int GetSize(int index) => _sizes[GetRootIndex(index)];
        public bool IsConnected(int first, int second) => Find(first) == Find(second);

        public void SetValue(int index, bool value)
        {
            _value[index] = value ? -1 : int.MaxValue;
            _sizes[index] = value ? Vector2Int.one : Vector2Int.zero;
        }

        public void Union(int first, int second, Vector2Int mergeDirection)
        {
            int firstSetRoot = Find(first);
            int secondSetRoot = Find(second);

            Vector2Int firstSize = _sizes[firstSetRoot];
            Vector2Int secondSize = _sizes[secondSetRoot];

            int firstRawSize = firstSize.x * firstSize.y;
            int secondRawSize = secondSize.x * secondSize.y;

            if (firstRawSize >= secondRawSize)
            {
                UpdateRoot(firstSetRoot, secondSetRoot, mergeDirection);
            }
            else
            {
                UpdateRoot(secondSetRoot, firstSetRoot, mergeDirection);
            }
        }

        private void UpdateRoot(int destinationIndex, int sourceIndex, Vector2Int mergeDirection)
        {
            mergeDirection += Vector2Int.one;

            _sizes[destinationIndex] = (_sizes[destinationIndex] + _sizes[sourceIndex]) * mergeDirection / 2;
            _value[sourceIndex] = destinationIndex;
        }

        private int Find(int index)
        {
            int rootIndex = GetRootIndex(index);
            CompressPath(index, rootIndex);

            return rootIndex;
        }

        public readonly int GetRootIndex(int rootIndex)
        {
            while (IsRoot(rootIndex) == false)
            {
                rootIndex = _value[rootIndex];
            }

            return rootIndex;
        }

        private void CompressPath(int index, int rootIndex)
        {
            while (IsRoot(rootIndex) == false)
            {
                int currentIndex = index;
                index = _value[index];

                _value[currentIndex] = rootIndex;
            }
        }

        public void Dispose()
        {
            _value.Dispose();
            _sizes.Dispose();
        }
    }
}