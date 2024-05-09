using Unity.Collections;
using Unity.Jobs;

namespace TerraformingTerrain2d
{
    public class MultiThreadedRectangularDecomposition
    {
        private readonly GridData _gridData;
        private readonly ChunkBounds _bounds;

        public MultiThreadedRectangularDecomposition(GridData gridData, ChunkBounds bounds)
        {
            _gridData = gridData;
            _bounds = bounds;
        }

        public TerraformingJobResult AddRectangles(in JobPersistentBuffers buffers, TerraformingJobResult terraformingResult)
        {
            NativeCounter triangleCounter = new(Allocator.TempJob);
            NativeCounter shapesCounter = new(Allocator.TempJob);
            CustomCollider2DPopulation colliderPopulation = new(buffers.ColliderVertices, buffers.Shapes,
                shapesCounter.ToConcurrent(), terraformingResult.PhysicsShapesCount);

            HorizontalMergeJob horizontalMergeJob = new(buffers.FullSquares, _bounds.Size);
            VerticalMergeJob verticalMergeJob = new(buffers.FullSquares, _bounds.Size);
            PopulateRectsFromDisjointSetJob rectsPopulationJob = new()
            {
                Bounds = _bounds,
                GridData = _gridData,
                Triangles = buffers.Triangles,
                DisjointSet = buffers.FullSquares,
                Collider2DPopulation = colliderPopulation,
                MarchingSquareResult = terraformingResult,
                TriangleCounter = triangleCounter.ToConcurrent(),
            };

            horizontalMergeJob.Schedule(_bounds.Size.y, 32).Complete();
            verticalMergeJob.Schedule(_bounds.Size.x, 32).Complete();
            rectsPopulationJob.Schedule(_bounds.Size.x * _bounds.Size.y, 32).Complete();

            TerraformingJobResult result = new(
                terraformingResult.TrianglesCount + triangleCounter.Count,
                terraformingResult.PhysicsShapesCount + shapesCounter.Count);

            shapesCounter.Dispose();
            triangleCounter.Dispose();

            return result;
        }
    }
}