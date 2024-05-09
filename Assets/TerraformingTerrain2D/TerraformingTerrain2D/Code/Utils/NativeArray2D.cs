using System;
using Unity.Collections;

namespace TerraformingTerrain2d
{
    public struct NativeArray2D<T> : IDisposable where T : struct
    {
        private NativeArray<T> _array;
        private readonly int _stride;

        public NativeArray2D(int dimensionX, int dimensionY, Allocator allocator)
        {
            _array = new NativeArray<T>(dimensionX * dimensionY, allocator);
            _stride = dimensionX;
        }

        public NativeArray<T> Array => _array;

        public T this[int i, int j]
        {
            get => _array[j * _stride + i];
            set => _array[j * _stride + i] = value;
        }

        public void Dispose()
        {
            _array.Dispose();
        }
    }
}