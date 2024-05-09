using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace TerraformingTerrain2d
{
    public abstract class Terraforming : IDisposable
    {
        protected readonly TerraformingData Data;

        protected Terraforming(TerraformingData data)
        {
            Data = data;
        }

        public void Initialize()
        {
            for (int i = Data.Bounds.Start.x, chunkX = 0; i < Data.Bounds.End.x; ++i, ++chunkX)
            {
                for (int j = Data.Bounds.Start.y, chunkY = 0; j < Data.Bounds.End.y; ++j, ++chunkY)
                {
                    Vector2 position = Data.GridData.GridToWorldPosition(i, j) + new Vector2(Data.GridData.Scale, Data.GridData.Scale) / 2f;
                    OnScalarFieldValueInitialization(chunkX, chunkY, new MarchingSquare(position, Data.GridData.Scale));
                }
            }
        }

        public void Rebuild()
        {
            Profiler.BeginSample("Rebuilding mesh...");
            Data.Collider.ClearCustomShapes();
            Clear();
            OnRebuild();
            Profiler.EndSample();
        }

        protected abstract void OnScalarFieldValueInitialization(int chunkX, int chunkY, MarchingSquare square);
        protected abstract void OnRebuild();
        protected virtual void Clear() { }
        public virtual void Dispose() { }
    }
}