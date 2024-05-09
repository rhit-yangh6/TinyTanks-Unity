using UnityEngine.Profiling;

namespace TerraformingTerrain2d
{
    public class SingleThreadedTerraforming : Terraforming
    {
        private readonly MeshConfigurationPresenter _configurationPresenter;
        private readonly SingleThreadedMeshPopulation _meshPopulation;
        private readonly GreedyMeshing _greedyMeshing;
        private readonly MarchingSquare[,] _grid;

        public SingleThreadedTerraforming(TerraformingData data) : base(data)
        {
            _grid = new MarchingSquare[data.ChunkSize.x, data.ChunkSize.y];
            _configurationPresenter = new MeshConfigurationPresenter(data.Input);
            _greedyMeshing = new GreedyMeshing(data.Bounds.Size, data.Input, data.GridData);
            _meshPopulation = new SingleThreadedMeshPopulation(data.MeshFilter, data.GridData.SquareDensity);
        }

        protected override void OnScalarFieldValueInitialization(int chunkX, int chunkY, MarchingSquare square)
        {
            _grid[chunkX, chunkY] = square;
        }

        protected override void Clear()
        {
            _greedyMeshing.VisitedSquares.Clear();
            Data.Collider.ClearShape();
            _meshPopulation.Clear();
        }

        protected override void OnRebuild()
        {
            RebuildMesh();
            Data.Collider.SetShapes();
            _meshPopulation.BufferData();
        }

        private void RebuildMesh()
        {
            for (int i = 0; i < Data.Iterations; ++i)
            {
                SquareIndex index = new(i, Data.Bounds);

                if (_greedyMeshing.VisitedSquares.IfCellWasProceed(index.ChunkX, index.ChunkY) == false)
                {
                    Data.Input.Update(index.GlobalX, index.GlobalY);

                    if (Data.Input.IsFullQuad)
                    {
                        Profiler.BeginSample("Greedy meshing...");
                        ProceedGreedyRectangle(in index);
                        Profiler.EndSample();
                    }
                    else
                    {
                        ProceedMarchingSquare(in index);
                    }
                }
            }
        }

        private void ProceedGreedyRectangle(in SquareIndex index)
        {
            RectangularDecompositionResult result = _greedyMeshing.GetGreedyRectangle(Data.Bounds.Start, index);
            _meshPopulation.AddRectangle(result, index.GlobalIndex);
            Data.Collider.AddRectangle(result.Min, result.Max);
        }

        private void ProceedMarchingSquare(in SquareIndex index)
        {
            MarchingSquareMeshConfiguration configuration = GetConfiguration(ref _grid[index.ChunkX, index.ChunkY]);
            _meshPopulation.AddMarchingSquare(configuration);
            Data.Collider.AddColliderPart(configuration.Vertices);
        }

        private MarchingSquareMeshConfiguration GetConfiguration(ref MarchingSquare square)
        {
            int configuration = Data.Input.GetConfiguration();
            square.Interpolate(Data.Input.ToImmutable());

            MarchingSquareMeshConfiguration meshConfiguration = _configurationPresenter.GetConfiguration(configuration);
            meshConfiguration.UpdateVertices(in square);

            return meshConfiguration;
        }
    }
}