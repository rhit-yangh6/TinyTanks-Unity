using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ChunksPresenter : IDisposable
    {
        private readonly IReadOnlyList<TerraformingTerrain2dChunk> _chunks;

        public ChunksPresenter(IReadOnlyList<TerraformingTerrain2dChunk> chunks)
        {
            _chunks = chunks;
        }

        public void TerraformAffectedChunks(Vector2 position, float radius)
        {
            for (int i = 0; i < _chunks.Count; ++i)
            {
                if (_chunks[i].Rectangle.IntersectsCircle(position, radius))
                {
                    _chunks[i].Rebuild();
                }
            }
        }

        public void UpdateColliderOffset()
        {
            for (int i = 0; i < _chunks.Count; ++i)
            {
                _chunks[i].UpdateColliderOffset();
            }
        }

        public void RebuildAll()
        {
            foreach (TerraformingTerrain2dChunk chunk in _chunks)
            {
                chunk.Rebuild();
            }
        }

        public void Dispose()
        {
            foreach (TerraformingTerrain2dChunk chunk in _chunks)
            {
                chunk.Dispose();
            }
        }
    }
}