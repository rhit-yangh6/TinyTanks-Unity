using UnityEngine;

namespace TerraformingTerrain2d
{
    public class GridMinMax
    {
        private readonly Vector2 _meshSize;

        public GridMinMax(GridData gridData)
        {
            _meshSize = gridData.GridToWorldPosition(gridData.SquareDensity);
        }

        public MinMax MinMax => new(Min, Max);
        public Vector2 Size => Max - Min;

        private Vector2 Max => _meshSize;
        private Vector2 Min => -_meshSize;
    }
}