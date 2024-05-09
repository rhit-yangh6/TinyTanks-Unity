using UnityEngine;

namespace TerraformingTerrain2d
{
    public class TerraformingFactory
    {
        private readonly TerraformingTerrainCollider _collider;
        private readonly ChunkSharedData _chunkData;
        private readonly MeshFilter _meshFilter;
        private readonly ChunkBounds _bounds;

        public TerraformingFactory(ChunkSharedData chunkData, TerraformingTerrainCollider collider, ChunkBounds bounds,
            MeshFilter meshFilter)
        {
            _meshFilter = meshFilter;
            _chunkData = chunkData;
            _collider = collider;
            _bounds = bounds;
        }

        public Terraforming Create()
        {
            TerraformingData data = new(_chunkData.GridData, _bounds, _chunkData.Input, _collider, _meshFilter);

            Terraforming terraforming = _chunkData.UseMultiThreading
                ? new MultiThreadedTerraforming(data)
                : new SingleThreadedTerraforming(data);

            terraforming.Initialize();

            return terraforming;
        }
    }
}