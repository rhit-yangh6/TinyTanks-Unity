using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ScalarFieldInitialization
    {
        private readonly IScalarFieldInitializer _maskScalarFieldInitializer;
        private readonly IScalarFieldInitializer _scalarFieldInitializer;
        private readonly ScalarFieldValue _value;
        private readonly Vector2Int _density;

        public ScalarFieldInitialization(ScalarFieldValue value, Vector2Int density, IScalarFieldInitializer scalarFieldInitializer, IScalarFieldInitializer maskScalarFieldInitializer)
        {
            _maskScalarFieldInitializer = maskScalarFieldInitializer;
            _scalarFieldInitializer = scalarFieldInitializer;
            _density = density;
            _value = value;
        }

        public void SetWithDefault()
        {
            for (int i = 0; i < _density.x; ++i)
            {
                for (int j = 0; j < _density.y; ++j)
                {
                    _value.Current[i, j] = _scalarFieldInitializer.Proceed(i, j, _density);
                    _value.Mask[i, j] = _maskScalarFieldInitializer.Proceed(i, j, _density);
                }
            }
        }
    }
}