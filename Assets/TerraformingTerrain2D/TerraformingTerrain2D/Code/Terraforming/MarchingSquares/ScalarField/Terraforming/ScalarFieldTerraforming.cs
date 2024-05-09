using UnityEngine;

namespace TerraformingTerrain2d
{
    public abstract class ScalarFieldTerraforming
    {
        private readonly ScalarFieldValue _value;
        private readonly GridData _gridData;
        private readonly IsoValue _isoValue;

        protected ScalarFieldTerraforming(GridData gridData, IsoValue isoValue, ScalarFieldValue value)
        {
            _gridData = gridData;
            _isoValue = isoValue;
            _value = value;
        }

        public void Execute(Vector2 position, float radius, TerraformingMode mode)
        {
            ScalarFieldTerraformingJob terraformingJob = CreateJob(position, radius, mode);
            int iterations = terraformingJob.Bound.Size.x * terraformingJob.Bound.Size.y;

            ProceedJob(ref terraformingJob, iterations);
        }

        private ScalarFieldTerraformingJob CreateJob(Vector2 position, float radius, TerraformingMode mode)
        {
            return new ScalarFieldTerraformingJob()
            {
                Radius = radius,
                Position = position,
                GridData = _gridData,
                TerraformingMode = mode,
                IsoValue = _isoValue.Value,
                ScalarField = _value.Current,
                Bound = CreateBounds(position, radius),
                OriginalScalarField = _value.Mask,
            };
        }

        private ChunkBounds CreateBounds(Vector2 position, float radius)
        {
            Vector2Int gridOrigin = _gridData.WorldToGridPosition(position);
            int gridRadius = (int)(2 * radius / _gridData.Scale) / 2;

            return new ChunkBounds(gridOrigin - Vector2Int.one * gridRadius,
                gridOrigin + Vector2Int.one * gridRadius);
        }

        protected abstract void ProceedJob(ref ScalarFieldTerraformingJob terraformingJob, int iterations);
    }
}