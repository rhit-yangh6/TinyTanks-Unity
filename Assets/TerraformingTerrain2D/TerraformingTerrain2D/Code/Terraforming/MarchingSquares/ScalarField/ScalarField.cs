using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public enum TerraformingMode
    {
        Fill,
        Carve,
    }

    public class ScalarField : IDisposable
    {
        private readonly ScalarFieldInitialization _fieldInitialization;
        private readonly ScalarFieldTerraforming _terraforming;
        private readonly ScalarFieldValue _value;

        public ScalarField(ScalarFieldValue value, ScalarFieldTerraforming terraforming, ScalarFieldInitialization fieldInitialization)
        {
            _fieldInitialization = fieldInitialization;
            _terraforming = terraforming;
            _value = value;
        }

        public NativeArray2D<float> Value => _value.Current;

        public float this[int i, int j] => _value.Current[i, j];

        public void SetWithDefault() => _fieldInitialization.SetWithDefault();

        public void Terraform(Vector2 localPosition, float radius, TerraformingMode mode) => _terraforming.Execute(localPosition, radius, mode);

        public void Dispose() => _value.Dispose();
    }
}