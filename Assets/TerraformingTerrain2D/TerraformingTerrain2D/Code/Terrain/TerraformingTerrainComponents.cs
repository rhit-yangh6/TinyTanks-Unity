using System;
using System.Collections.Generic;

namespace TerraformingTerrain2d
{
    public class TerraformingTerrainComponents : IDisposable
    {
        public readonly TerraformingPresenter TerraformingPresenter;
        public readonly TerraformingTerrain2DOutline Outline;
        public readonly TransformUpdate TransformUpdate;
        public readonly ChunksPresenter ChunksPresenter;
        public readonly ChunksHolder ChunksHolder;
        public readonly ScalarField ScalarField;
        public readonly GridMinMax GridMinMax;
        public readonly TerrainGizmo Gizmo;
        private bool _wasDisposed;

        public TerraformingTerrainComponents(TerraformingTerrainData data)
        {
            GridData gridData = new(data.Density, data.SdfTexture.GetRatio(), data.Scale);
            ScalarField = new ScalarFieldFactory(data, gridData).Create();
            GridMinMax = new GridMinMax(gridData);

            List<TerraformingTerrain2dChunk> chunks = new();
            ChunksPresenter = new ChunksPresenter(chunks);
            Outline = new OutlineFactory(data.Transform, data.SdfTexture.GetResolution()).Create();
            TerraformingPresenter = new TerraformingPresenter(Outline, ChunksPresenter, ScalarField, GridMinMax, data.Transform);

            MarchingSquareInput input = new(ScalarField, gridData.SquareDensity, data.IsoValue);
            ChunkSharedData chunkSharedData = new(gridData, input, data.Input.Material, data.Transform, data.UseMultiThreading, TerraformingPresenter, data.SortingOrder);
            ChunkBoundsCalculator chunkBoundsCalculator = new(data.GeneratedChunksCount, gridData.Density);

            ChunksHolder = new ChunksHolder(chunkSharedData, chunkBoundsCalculator, chunks);
            TransformUpdate = new TransformUpdate(ChunksPresenter, data.Transform);
            Gizmo = new TerrainGizmo(data.GizmoData, ScalarField, gridData, data.IsoValue);
        }

        public void Dispose()
        {
            CleanUp();
            GC.SuppressFinalize(this);
        }

        ~TerraformingTerrainComponents()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (_wasDisposed == false)
            {
                _wasDisposed = true;
                ScalarField.Dispose();
                ChunksPresenter.Dispose();    
            }
        }
    }
}