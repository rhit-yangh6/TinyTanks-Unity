using UnityEngine;

namespace TerraformingTerrain2d
{
    public class GreedyRectangleSearch
    {
        private readonly VisitedSquares _visitedSquares;
        private readonly MarchingSquareInput _input;
        private readonly Vector2Int _size;
        private Vector2Int _chunkOrigin;

        public GreedyRectangleSearch(MarchingSquareInput input, VisitedSquares visitedSquares, Vector2Int size)
        {
            _visitedSquares = visitedSquares;
            _input = input;
            _size = size;
        }

        public Vector2Int GetRectangle(Vector2Int chunkOrigin, Vector2Int chunkPosition)
        {
            _chunkOrigin = chunkOrigin;
            int maxX = GetRectangleWidth(chunkPosition.x, chunkPosition.y);
            int maxY = GetRectangleHeight(chunkPosition, maxX);

            return new Vector2Int()
            {
                x = maxX,
                y = maxY
            };
        }

        private int GetRectangleWidth(int i, int j)
        {
            while (i + 1 < _size.x)
            {
                if (IsSquareValidToMerge(i + 1, j))
                {
                    ++i;
                }
                else
                {
                    break;
                }
            }

            return i;
        }

        private int GetRectangleHeight(Vector2Int gridPosition, int maxX)
        {
            int y = gridPosition.y + 1;

            for (; y < _size.y; ++y)
            {
                for (int x = gridPosition.x; x < maxX + 1; x++)
                {
                    if (IsSquareValidToMerge(x, y) == false)
                    {
                        return y - 1;
                    }
                }
            }

            return y - 1;
        }

        private bool IsSquareValidToMerge(int i, int j)
        {
            _input.Update(_chunkOrigin.x + i, _chunkOrigin.y + j);

            return _input.IsFullQuad && _visitedSquares.IfCellWasProceed(i, j) == false;
        }
    }
}