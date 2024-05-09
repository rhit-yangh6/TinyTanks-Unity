using System;
using Unity.Collections;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ScalarFieldValue : IDisposable
    {
        public NativeArray2D<float> Current;
        public NativeArray2D<float> Mask;

        public ScalarFieldValue(Vector2Int density)
        {
            Mask = new NativeArray2D<float>(density.x, density.y, Allocator.Persistent);
            Current = new NativeArray2D<float>(density.x, density.y, Allocator.Persistent);
        }

        public void Dispose()
        {
            Current.Dispose();
            Mask.Dispose();
        }
    }
}