using System;
using Unity.Collections;
using Unity.Jobs;

namespace TerraformingTerrain2d
{
    public class MarchingSquareTerraformingJobLauncher : IDisposable
    {
        private NativeMarchingSquareConfigurationsPresenter _configurations;
        private readonly TerraformingData _data;

        public MarchingSquareTerraformingJobLauncher(TerraformingData data)
        {
            _configurations = NativeMarchingSquareConfigurationsFactory.Create();
            _data = data;
        }

        public TerraformingJobResult Start(in JobPersistentBuffers buffers)
        {
            NativeCounter trisCounter = new(Allocator.TempJob);
            NativeCounter shapesCounter = new(Allocator.TempJob);
            MarchingSquareTerraformingJobData data = new(_data.Bounds, _data.GridData.Density, _data.Input.IsoValue,
                _configurations);
            MarchingSquareTerraformingJob terraformingJob = new()
            {
                Data = data,
                Grid = buffers.Grid,
                Triangles = buffers.Triangles,
                FullSquares = buffers.FullSquares,
                ScalarField = _data.Input.ScalarField.Value,
                TrianglesCounter = trisCounter.ToConcurrent(),
                ColliderPopulation = new CustomCollider2DPopulation(buffers.ColliderVertices, buffers.Shapes,
                    shapesCounter.ToConcurrent()),
            };

            terraformingJob.Schedule(_data.Iterations, 32).Complete();

            TerraformingJobResult result = new(trisCounter.Count, shapesCounter.Count);
            shapesCounter.Dispose();
            trisCounter.Dispose();

            return result;
        }

        public void Dispose()
        {
            _configurations.Dispose();
        }
    }
}