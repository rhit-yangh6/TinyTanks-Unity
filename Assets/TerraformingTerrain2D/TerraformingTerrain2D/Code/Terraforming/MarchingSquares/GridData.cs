using UnityEngine;

namespace TerraformingTerrain2d
{
    public readonly struct GridData
    {
        private readonly int _density;
        private readonly float _ratio;
        private readonly float _scale;

        public GridData(int density, float ratio, float scale)
        {
            _density = density;
            _ratio = ratio;
            _scale = scale;
        }

        public int XDensity => Mathf.CeilToInt(YDensity * _ratio);
        public int YDensity => _density + 1;

        public Vector2Int Density => new(XDensity, YDensity);
        public float Scale => (float)YDensity / (XDensity * YDensity) * _scale;
        public Vector2Int SquareDensity => Density - Vector2Int.one;

        public Vector2Int WorldToGridPosition(Vector2 worldPosition)
        {
            Vector2Int gridPositions = new();

            gridPositions.x = Mathf.FloorToInt((worldPosition.x + XDensity * Scale / 2f) / Scale);
            gridPositions.y = Mathf.FloorToInt((worldPosition.y + YDensity * Scale / 2f) / Scale);

            return gridPositions;
        }

        public Vector2 GridToWorldPosition(Vector2Int gridPosition)
        {
            return GridToWorldPosition(gridPosition.x, gridPosition.y);
        }

        public Vector2 GridToWorldPosition(int i, int j)
        {
            Vector2 worldPosition = new Vector2(i, j) * Scale;

            worldPosition.x -= XDensity * Scale / 2f - Scale / 2f;
            worldPosition.y -= YDensity * Scale / 2f - Scale / 2f;

            return worldPosition;
        }

        public bool IsInBounds(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < XDensity &&
                   gridPosition.y >= 0 && gridPosition.y < YDensity;
        }
    }
}