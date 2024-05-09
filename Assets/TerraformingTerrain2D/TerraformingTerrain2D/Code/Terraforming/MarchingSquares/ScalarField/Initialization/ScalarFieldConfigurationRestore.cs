using System.Collections.Generic;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ScalarFieldConfigurationRestore : IScalarFieldInitializer
    {
        private readonly IReadOnlyList<float> _configurationScalarField;

        public ScalarFieldConfigurationRestore(IReadOnlyList<float> configurationScalarField)
        {
            _configurationScalarField = configurationScalarField;
        }

        public float Proceed(int x, int y, Vector2Int density)
        {
            return _configurationScalarField[y * density.x + x];
        }
    }
}