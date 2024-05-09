using System;
using Unity.Collections;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public struct JobPersistentBuffers : IDisposable
    {
        public NativeArray<Vector2> ColliderVertices;
        public NativeArray<PhysicsShape2D> Shapes;
        public NativeArray<MarchingSquare> Grid;
        public NativeArray<TerrainVertex> Triangles;
        public DisjointSet FullSquares;

        public JobPersistentBuffers(int chunkSize)
        {
            ColliderVertices = new NativeArray<Vector2>(chunkSize * 6, Allocator.Persistent);
            Triangles = new NativeArray<TerrainVertex>(chunkSize * 12, Allocator.Persistent); // marching squares has max 4 triangles
            Shapes = new NativeArray<PhysicsShape2D>(chunkSize, Allocator.Persistent);
            Grid = new NativeArray<MarchingSquare>(chunkSize, Allocator.Persistent);
            FullSquares = new DisjointSet(chunkSize, Allocator.Persistent);
        }

        public void Dispose()
        {
            ColliderVertices.Dispose();
            FullSquares.Dispose();
            Triangles.Dispose();
            Shapes.Dispose();
            Grid.Dispose();
        }
    }
}