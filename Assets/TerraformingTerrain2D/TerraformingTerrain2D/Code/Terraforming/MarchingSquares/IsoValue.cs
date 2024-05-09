using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [Serializable]
    public class IsoValue
    {
        [SerializeField] private float _isoValue = 1.1f;

        public float Value => _isoValue;
    }
}