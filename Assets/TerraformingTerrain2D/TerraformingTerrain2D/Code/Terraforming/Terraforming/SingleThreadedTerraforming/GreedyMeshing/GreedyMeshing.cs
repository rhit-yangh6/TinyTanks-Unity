using UnityEngine;

namespace TerraformingTerrain2d
{
    public class GreedyMeshing
    {
        private readonly GreedyRectangleSearch _greedyRectangleSearch;
        public readonly VisitedSquares VisitedSquares;
        private readonly GridData _gridData;

        public GreedyMeshing(Vector2Int chunkSize, MarchingSquareInput input, GridData gridData)
        {
            _gridData = gridData;
            VisitedSquares = new VisitedSquares(chunkSize);
            _greedyRectangleSearch = new GreedyRectangleSearch(input, VisitedSquares, chunkSize);
        }

        public RectangularDecompositionResult GetGreedyRectangle(Vector2Int chunkOrigin, in SquareIndex index)
        {
            Vector2Int rectangle = _greedyRectangleSearch.GetRectangle(chunkOrigin, index.ChunkIndex);

            Vector2 min = _gridData.GridToWorldPosition(index.GlobalIndex.x, index.GlobalIndex.y);
            Vector2 max = _gridData.GridToWorldPosition(chunkOrigin + rectangle + Vector2Int.one);

            VisitedSquares.MarkRectangleAsVisited(index.ChunkIndex.x, index.ChunkIndex.y, rectangle.x, rectangle.y);

            return new RectangularDecompositionResult(chunkOrigin + rectangle + Vector2Int.one, min, max);
        }
    }
}