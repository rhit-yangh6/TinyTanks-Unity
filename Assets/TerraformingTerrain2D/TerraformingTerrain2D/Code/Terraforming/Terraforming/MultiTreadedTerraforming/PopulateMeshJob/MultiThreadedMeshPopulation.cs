using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class MultiThreadedMeshPopulation
    {
        private readonly NativeArrayMeshPopulation<TerrainVertex> _meshPopulation;

        public MultiThreadedMeshPopulation(MeshFilter meshFilter)
        {
            _meshPopulation = new NativeArrayMeshPopulation<TerrainVertex>(meshFilter);
        }

        public void Populate(in NativeArray<TerrainVertex> triangles, int trisCount)
        {
            NativeParallelHashSet<TerrainVertex> distinctVertices = new(trisCount * 3, Allocator.TempJob);
            new DistinctVerticesJob(triangles, distinctVertices.AsParallelWriter())
                .Schedule(trisCount * 3, 32)
                .Complete();

            NativeArray<TerrainVertex> vertices = distinctVertices.ToNativeArray(Allocator.TempJob);
            NativeParallelHashMap<TerrainVertex, int> indicesHasMap = new(vertices.Length, Allocator.TempJob);
            new PopulateIndicesJob(vertices, indicesHasMap.AsParallelWriter())
                .Schedule(distinctVertices.Count(), 32)
                .Complete();

            NativeArray<int> indices = new(trisCount * 3, Allocator.TempJob);
            new PopulateTrianglesJob(triangles, indicesHasMap, indices)
                .Schedule(trisCount, 32)
                .Complete();

            _meshPopulation.SetMeshData(vertices, indices);
            _meshPopulation.BufferData();

            indices.Dispose();
            vertices.Dispose();
            indicesHasMap.Dispose();
            distinctVertices.Dispose();
        }
    }
}