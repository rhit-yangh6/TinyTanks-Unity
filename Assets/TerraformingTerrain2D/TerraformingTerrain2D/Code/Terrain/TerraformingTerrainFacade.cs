using UnityEngine;

namespace TerraformingTerrain2d
{
    public class TerraformingTerrainFacade
    {
        private TerraformingTerrainComponents _components;
        private readonly TerraformingTerrainData _data;

        public TerraformingTerrainFacade(TerraformingTerrainData data)
        {
            _data = data;
        }

        public void Terraform(Vector2 position, float radius, TerraformingMode mode)
        {
            _components.TerraformingPresenter.Rebuild(position, radius, mode);
        }
        
        public void Clear()
        {
            _components.ScalarField.SetWithDefault();
            _components.ChunksPresenter.RebuildAll();
            _components.Outline.Clear();
        }

        public void Regenerate()
        {
            _components = RecreateComponents();
            _components.ChunksHolder.DestroyChunks();
            _components.ChunksHolder.InstantiateNewChunks();
            RebuildTerrain();
        }
        
        private void RebuildTerrain()
        {
            _components.ScalarField.SetWithDefault();
            _components.ChunksHolder.ComposeChunks();
            _components.ChunksPresenter.RebuildAll();
        }

        public void ReassembleChunks()
        {
            _components = RecreateComponents();
            _components.ChunksHolder.CollectFromChildren();
            _components.Outline.RebuildRectangle(_components.GridMinMax.Size);
            _components.Outline.Setup();
            RebuildTerrain();
        }

        public void UpdateTransform()
        {
            _components?.TransformUpdate.Update();
        }

        public void Dispose()
        {
            _components?.Dispose();
        }

        private TerraformingTerrainComponents RecreateComponents()
        {
            Dispose();
            return new TerraformingTerrainComponents(_data);
        }
    }
}