using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [BurstCompile]
    public struct PopulateRectsFromDisjointSetJob : IJobParallelFor
    {
        [ReadOnly] public TerraformingJobResult MarchingSquareResult;
        [ReadOnly] public DisjointSet DisjointSet;
        [ReadOnly] public ChunkBounds Bounds;
        [ReadOnly] public GridData GridData;

        [NativeDisableContainerSafetyRestriction] [WriteOnly]
        public NativeArray<TerrainVertex> Triangles;

        [WriteOnly] public NativeCounter.Concurrent TriangleCounter;
        [WriteOnly] public CustomCollider2DPopulation Collider2DPopulation;

        [BurstCompile]
        public void Execute(int i)
        {
            if (DisjointSet.IsRoot(i))
            {
                SquareIndex index = new(i, Bounds);
                Vector2Int size = DisjointSet.GetSize(i);

                Vector2 min = GridData.GridToWorldPosition(index.GlobalIndex.x, index.GlobalIndex.y);
                Vector2 max = GridData.GridToWorldPosition(index.GlobalIndex + size);

                DecomposedRectangleCalculator calculator = new(GridData.SquareDensity, index.GlobalIndex,
                    stackalloc TerrainVertex[6]);
                RectangularDecompositionResult decompositionResult = new(index.GlobalIndex + size, min, max);
                calculator.Evaluate(decompositionResult);

                PopulateRectangleToMesh(in calculator);
                PopulateRectangleToCollider(in calculator);
            }
        }

        private void PopulateRectangleToMesh(in DecomposedRectangleCalculator calculator)
        {
            for (int rectangleTriangle = 0; rectangleTriangle < 2; ++rectangleTriangle)
            {
                int triangleIndex = (MarchingSquareResult.TrianglesCount + TriangleCounter.Increment()) * 3;

                for (int vertexIndex = 0; vertexIndex < 3; ++vertexIndex)
                {
                    int rectangleVertexIndex = vertexIndex + rectangleTriangle * 3;
                    Triangles[triangleIndex + vertexIndex] = calculator.Triangles[rectangleVertexIndex];
                }
            }
        }

        private void PopulateRectangleToCollider(in DecomposedRectangleCalculator rectangleCalculator)
        {
            Span<Vector2> vertices = stackalloc Vector2[4];
            rectangleCalculator.GetVertices(ref vertices);

            Collider2DPopulation.AddCollider(in vertices);
        }
    }
}