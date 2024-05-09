using System.Collections.Generic;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ChunksHolder
    {
        private readonly ChunkBoundsCalculator _chunkBoundsCalculator;
        private readonly List<TerraformingTerrain2dChunk> _chunks;
        private readonly ChunkSharedData _chunkSharedData;

        public ChunksHolder(ChunkSharedData chunkSharedData, ChunkBoundsCalculator chunkBoundsCalculator, List<TerraformingTerrain2dChunk> chunks)
        {
            _chunkBoundsCalculator = chunkBoundsCalculator;
            _chunkSharedData = chunkSharedData;
            _chunks = chunks;
        }

        public void CollectFromChildren()
        {
            _chunks.Clear();

            foreach (Transform child in _chunkSharedData.Parent.transform)
            {
                if (child.TryGetComponent(out TerraformingTerrain2dChunk chunk2D))
                {
                    _chunks.Add(chunk2D);
                }
            }
        }

        public void ComposeChunks()
        {
            for (int i = 0; i < _chunks.Count; ++i)
            {
                int x = i % _chunkBoundsCalculator.GeneratedChunksCount.x;
                int y = i / _chunkBoundsCalculator.GeneratedChunksCount.x;

                _chunks[i].Compose(_chunkSharedData, _chunkBoundsCalculator.Evaluate(x, y));
            }
        }

        public void DestroyChunks()
        {
            TerraformingTerrain2dChunk[] chunks =
                _chunkSharedData.Parent.GetComponentsInChildren<TerraformingTerrain2dChunk>(true);

            for (int i = 0; i < chunks.Length; ++i)
            {
                Object.DestroyImmediate(chunks[i].gameObject);
            }
        }

        public void InstantiateNewChunks()
        {
            _chunks.Clear();

            for (int i = 0; i < _chunkBoundsCalculator.GeneratedChunksCount.y; ++i)
            {
                for (int j = 0; j < _chunkBoundsCalculator.GeneratedChunksCount.x; ++j)
                {
                    TerraformingTerrain2dChunk chunk = InstantiateChunk(i, j);
                    _chunks.Add(chunk);
                }
            }
        }

        private TerraformingTerrain2dChunk InstantiateChunk(int i, int j)
        {
            GameObject chunk = new();
            chunk.name = $"{j}, {i}";
            chunk.transform.parent = _chunkSharedData.Parent;
            chunk.transform.localScale = Vector3.one;
            chunk.transform.localPosition = Vector3.zero;
            chunk.transform.localRotation = Quaternion.identity;
            chunk.gameObject.layer = _chunkSharedData.Parent.gameObject.layer;

            return chunk.AddComponent<TerraformingTerrain2dChunk>();
        }
    }
}