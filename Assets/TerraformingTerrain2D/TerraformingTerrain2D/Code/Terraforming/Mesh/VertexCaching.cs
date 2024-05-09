using System.Collections.Generic;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class VertexCaching
    {
        private readonly Dictionary<TerrainVertex, int> _cachedVertices = new();
        private readonly List<TerrainVertex> _terrainVertices = new();

        public List<TerrainVertex> Vertices => _terrainVertices;

        public void Clear()
        {
            _cachedVertices.Clear();
            _terrainVertices.Clear();
        }

        public int GetIndex(TerrainVertex cachedVertex)
        {
            return _cachedVertices[cachedVertex];
        }

        public void TryAdd(TerrainVertex terrainVertex)
        {
            if (_cachedVertices.ContainsKey(terrainVertex) == false)
            {
                int index = _cachedVertices.Count;

                _cachedVertices.Add(terrainVertex, index);
                _terrainVertices.Add(terrainVertex);
            }
        }

        public void TryAdd(Vector2 terrainVertex, Vector2 vector2)
        {
            TryAdd(new TerrainVertex(terrainVertex, vector2));
        }
    }
}