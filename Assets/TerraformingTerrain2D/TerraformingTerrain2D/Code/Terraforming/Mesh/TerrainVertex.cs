using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TerrainVertex : IEquatable<TerrainVertex>
    {
        public readonly Vector3 Position;
        public readonly Vector2 UV;
        private static readonly int _comparisonPrecision = 3;

        public TerrainVertex(Vector3 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }

        public bool Equals(TerrainVertex other)
        {
            return Position.Round(_comparisonPrecision) == other.Position.Round(_comparisonPrecision);
        }

        public override bool Equals(object obj)
        {
            return obj is TerrainVertex other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Position.Round(_comparisonPrecision).GetHashCode();
        }
    }
}