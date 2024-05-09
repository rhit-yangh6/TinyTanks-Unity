using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class GridConfiguration : ScriptableObject
    {
        [SerializeField] [ShowOnly] Texture2D _sdfTexture;
        [SerializeField] [ShowOnly] private float _sdfFactor;
        [SerializeField] [ShowOnly] private int _density;
        [SerializeField, HideInInspector] private float[] _scalarField;

        public Texture2D SDFTexture => _sdfTexture;
        public float SDFFactor => _sdfFactor;
        public int Density => _density;
        public IReadOnlyList<float> ScalarField => _scalarField;

        public void Initialize(Texture2D sdfTexture, float sdfFactor, int density, NativeArray<float> scalarField)
        {
            _sdfTexture = sdfTexture;
            _sdfFactor = sdfFactor;
            _density = density;
            _scalarField = scalarField.ToArray();
        }
    }
}