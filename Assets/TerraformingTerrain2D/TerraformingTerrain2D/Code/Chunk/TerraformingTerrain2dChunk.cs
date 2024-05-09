using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [RequireComponent(typeof(CustomCollider2D), typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerraformingTerrain2dChunk : MonoBehaviour, IDisposable
    {
        private TerraformingTerrainCollider _terrainCollider;
        private Terraforming _terraforming;

        public TerraformingPresenter TerraformingPresenter { get; private set; }
        public Rectangle Rectangle { get; private set; }

        public void Compose(ChunkSharedData data, ChunkBounds bounds)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            CustomCollider2D customCollider = GetComponent<CustomCollider2D>();
            _terrainCollider = new TerraformingTerrainCollider(customCollider);
            TerraformingFactory terraformingFactory = new(data, _terrainCollider, bounds, meshFilter);
            _terraforming = terraformingFactory.Create();
            TerraformingPresenter = data.TerraformingPresenter;

            SetRect(data.GridData, bounds);
            SetBounds(meshFilter, data.GridData, bounds);
            SetupMeshRenderer(meshRenderer, data.Material, data.SortingOrder);
        }

        private void SetRect(GridData gridData, ChunkBounds chunkBounds)
        {
            Vector2 start = gridData.GridToWorldPosition(chunkBounds.Start);
            Vector2 end = gridData.GridToWorldPosition(chunkBounds.End);
            Rect rect = new(start, end - start);

            Rectangle = new Rectangle(rect, transform);
        }

        private void SetBounds(MeshFilter meshFilter, GridData gridData, ChunkBounds bounds)
        {
            Vector2 start = gridData.GridToWorldPosition(bounds.Start);
            Vector2 end = gridData.GridToWorldPosition(bounds.End);

            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.bounds = new Bounds((start + end) / 2f, end - start);
        }

        private void SetupMeshRenderer(MeshRenderer meshRenderer, Material material, int sortingOrder)
        {
            meshRenderer.material = material;
            meshRenderer.sortingOrder = sortingOrder;
        }

        public void UpdateColliderOffset()
        {
            _terrainCollider?.UpdateOffset();
        }

        public void Rebuild()
        {
            _terraforming.Rebuild();
        }

        private void OnDrawGizmos()
        {
            //Rectangle?.DrawGizmo();
        }

        public void Dispose()
        {
            _terraforming?.Dispose();
        }
    }
}