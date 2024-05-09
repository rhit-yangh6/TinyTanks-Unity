using UnityEngine;

namespace TerraformingTerrain2d
{
    public class VisitedSquares
    {
        private readonly bool[,] _fullQuadGrid;
        private readonly Vector2Int _size;

        public VisitedSquares(Vector2Int size)
        {
            _fullQuadGrid = new bool[size.x, size.y];
            _size = size;
        }

        public void Clear()
        {
            for (int i = 0; i < _size.x; ++i)
            {
                for (int j = 0; j < _size.y; ++j)
                {
                    _fullQuadGrid[i, j] = false;
                }
            }
        }

        public bool IfCellWasProceed(int i, int j)
        {
            return _fullQuadGrid[i, j];
        }

        public void MarkRectangleAsVisited(int startX, int startY, int maxX, int maxY)
        {
            for (int i = startX; i <= maxX; ++i)
            {
                for (int j = startY; j <= maxY; ++j)
                {
                    _fullQuadGrid[i, j] = true;
                }
            }
        }
    }
}