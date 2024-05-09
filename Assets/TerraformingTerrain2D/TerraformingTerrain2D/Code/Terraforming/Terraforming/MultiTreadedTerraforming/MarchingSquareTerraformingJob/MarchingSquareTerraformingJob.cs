using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [BurstCompile]
    public struct MarchingSquareTerraformingJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] [ReadOnly]
        public NativeArray<MarchingSquare> Grid;

        [ReadOnly] public MarchingSquareTerraformingJobData Data;
        [ReadOnly] public NativeArray2D<float> ScalarField;

        [WriteOnly] public CustomCollider2DPopulation ColliderPopulation;

        [NativeDisableParallelForRestriction] [WriteOnly]
        public NativeArray<TerrainVertex> Triangles;

        [WriteOnly] public NativeCounter.Concurrent TrianglesCounter;
        [WriteOnly] public DisjointSet FullSquares;

        [BurstCompile]
        public void Execute(int index)
        {
            SquareIndex squareIndex = new(index, Data.Bounds);

            MarchingSquareImmutableInput immutableInput = GetInput(squareIndex.GlobalX, squareIndex.GlobalY);
            FullSquares.SetValue(index, immutableInput.IsFullQuad);

            if (immutableInput.IsFullQuad == false)
            {
                AddMarchingSquarePart(in immutableInput, in squareIndex);
            }
        }

        private MarchingSquareImmutableInput GetInput(int i, int j)
        {
            return new MarchingSquareImmutableInput(Data.IsoValue,
                ScalarField[i + 1, j + 1],
                ScalarField[i + 1, j],
                ScalarField[i, j],
                ScalarField[i, j + 1]);
        }

        private void AddMarchingSquarePart(in MarchingSquareImmutableInput immutableInput, in SquareIndex squareIndex)
        {
            MarchingSquare square = Grid[squareIndex.ChunkY * Data.Bounds.Size.x + squareIndex.ChunkX];

            int configurationIndex = immutableInput.GetConfiguration();
            square.Interpolate(immutableInput);

            NativeMarchingSquareMeshConfiguration configuration = Data.Configurations[configurationIndex];
            Span<Vector2> vertices = stackalloc Vector2[configuration.VerticesCount];
            Span<Vector2> uv = stackalloc Vector2[configuration.VerticesCount];

            Data.SquareFunctionsList.UpdateVertices(configurationIndex, ref vertices, ref uv, in square,
                squareIndex.GlobalIndex, Data.Density - Vector2Int.one);

            AddPolygon(in configuration, in vertices, in uv);
            ColliderPopulation.AddCollider(in vertices);
        }

        private void AddPolygon(in NativeMarchingSquareMeshConfiguration configuration, in Span<Vector2> vertices,
            in Span<Vector2> uv)
        {
            for (int i = 0; i < configuration.Triangles.Length; i += 3)
            {
                int firstIndex = configuration.Triangles[i];
                int secondIndex = configuration.Triangles[i + 1];
                int thirdIndex = configuration.Triangles[i + 2];
                int triangleIndex = TrianglesCounter.Increment() * 3;

                Triangles[triangleIndex] = new TerrainVertex(vertices[firstIndex], uv[firstIndex]);
                Triangles[triangleIndex + 1] = new TerrainVertex(vertices[secondIndex], uv[secondIndex]);
                Triangles[triangleIndex + 2] = new TerrainVertex(vertices[thirdIndex], uv[thirdIndex]);
            }
        }
    }
}