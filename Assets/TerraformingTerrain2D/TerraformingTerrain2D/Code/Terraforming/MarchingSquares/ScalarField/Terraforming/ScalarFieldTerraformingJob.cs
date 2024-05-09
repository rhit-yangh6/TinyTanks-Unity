using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public struct ScalarFieldTerraformingJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeArray2D<float> OriginalScalarField;
        [NativeDisableParallelForRestriction] public NativeArray2D<float> ScalarField;
        public TerraformingMode TerraformingMode;
        public GridData GridData;
        public ChunkBounds Bound;
        public Vector2 Position;
        public float IsoValue;
        public float Radius;

        public void Execute(int index)
        {
            Vector2Int gridPosition = new()
            {
                x = index % Bound.Size.x + Bound.Start.x,
                y = index / Bound.Size.x + Bound.Start.y
            };

            if (GridData.IsInBounds(gridPosition))
            {
                ProceedGridPoint(gridPosition);
            }
        }

        private void ProceedGridPoint(Vector2Int gridPosition)
        {
            Vector2 worldPosition = GridData.GridToWorldPosition(gridPosition.x, gridPosition.y);
            float distance = Vector2.Distance(Position, worldPosition);

            if (distance < Radius)
            {
                if (TerraformingMode == TerraformingMode.Carve)
                {
                    Carve(gridPosition, distance);
                }

                else if (TerraformingMode == TerraformingMode.Fill)
                {
                    Fill(gridPosition, distance);
                }
            }
        }

        private void Carve(Vector2Int gridPosition, float distance)
        {
            float factor = Radius / distance / 2f;
            factor = Mathf.LerpUnclamped(0, IsoValue, factor);

            float oldValue = ScalarField[gridPosition.x, gridPosition.y];
            ScalarField[gridPosition.x, gridPosition.y] = Mathf.Max(Mathf.Max(0, factor), oldValue);
        }

        private void Fill(Vector2Int gridPosition, float distance)
        {
            float factor = distance / Radius * 2f;
            factor = Mathf.LerpUnclamped(0, IsoValue, factor);

            float oldValue = ScalarField[gridPosition.x, gridPosition.y];
            float originalValue = OriginalScalarField[gridPosition.x, gridPosition.y];
            ScalarField[gridPosition.x, gridPosition.y] = Mathf.Max(Mathf.Min(factor, oldValue), originalValue);
        }
    }
}