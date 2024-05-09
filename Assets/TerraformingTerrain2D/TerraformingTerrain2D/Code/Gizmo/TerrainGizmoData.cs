using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [Serializable]
    public class TerrainGizmoData
    {
        [SerializeField] private bool _showSpheres = false;
        [SerializeField] private bool _showValues = false;
        [SerializeField] [Min(0)] private float _sphereRadius = 0.075f;

        public bool ShowSpheres => _showSpheres;
        public bool ShowValues => _showValues;
        public float SphereRadius => _sphereRadius;
    }
}