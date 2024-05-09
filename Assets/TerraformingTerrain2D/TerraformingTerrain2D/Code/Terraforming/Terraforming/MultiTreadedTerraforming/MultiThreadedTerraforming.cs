using UnityEngine.Profiling;

namespace TerraformingTerrain2d
{
    public class MultiThreadedTerraforming : Terraforming
    {
        private readonly MultiThreadedRectangularDecomposition _rectangularDecomposition;
        private readonly MarchingSquareTerraformingJobLauncher _terraformingJobLauncher;
        private readonly MultiThreadedMeshPopulation _meshPopulation;
        private JobPersistentBuffers _buffers;

        public MultiThreadedTerraforming(TerraformingData data) : base(data)
        {
            _rectangularDecomposition = new MultiThreadedRectangularDecomposition(Data.GridData, Data.Bounds);
            _terraformingJobLauncher = new MarchingSquareTerraformingJobLauncher(Data);
            _meshPopulation = new MultiThreadedMeshPopulation(Data.MeshFilter);
            _buffers = new JobPersistentBuffers(Data.Iterations);
        }

        protected override void OnScalarFieldValueInitialization(int chunkX, int chunkY, MarchingSquare square)
        {
            _buffers.Grid[chunkY * Data.ChunkSize.x + chunkX] = square;
        }

        protected override void OnRebuild()
        {
            Profiler.BeginSample("Marching squares...");
            TerraformingJobResult result = _terraformingJobLauncher.Start(in _buffers);
            Profiler.EndSample();

            Profiler.BeginSample("Rectangular decomposition...");
            result = _rectangularDecomposition.AddRectangles(in _buffers, result);
            Profiler.EndSample();

            Profiler.BeginSample("Populating mesh...");
            _meshPopulation.Populate(in _buffers.Triangles, result.TrianglesCount);
            Profiler.EndSample();

            Profiler.BeginSample("Updating collider...");
            Data.Collider.SetShapes(_buffers.Shapes, _buffers.ColliderVertices, result.PhysicsShapesCount);
            Profiler.EndSample();
        }

        public override void Dispose()
        {
            _terraformingJobLauncher.Dispose();
            _buffers.Dispose();
        }
    }
}