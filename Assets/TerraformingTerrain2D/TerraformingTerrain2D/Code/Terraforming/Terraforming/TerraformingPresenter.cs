using UnityEngine;
using UnityEngine.Profiling;

namespace TerraformingTerrain2d
{
    public class TerraformingPresenter
    {
        private readonly TerraformingTerrain2DOutline _outline;
        private readonly ChunksPresenter _chunksPresenter;
        private readonly ScalarField _scalarField;
        private readonly GridMinMax _gridMinMax;
        private readonly Transform _transform;

        public TerraformingPresenter(TerraformingTerrain2DOutline outline, ChunksPresenter chunksPresenter, ScalarField scalarField, GridMinMax gridMinMax, Transform transform)
        {
            _chunksPresenter = chunksPresenter;
            _scalarField = scalarField;
            _gridMinMax = gridMinMax;
            _transform = transform;
            _outline = outline;
        }

        public void Rebuild(Vector2 position, float radius, TerraformingMode mode)
        {
            Vector2 localPosition = _transform.InverseTransformPoint(position);
            
            Profiler.BeginSample("Scalar field terraform...");
            _scalarField.Terraform(localPosition, radius, mode);
            Profiler.EndSample();

            Profiler.BeginSample("Regenerating chunks...");
            _chunksPresenter.TerraformAffectedChunks(position, radius / 2f);
            Profiler.EndSample();

            if (Application.isPlaying)
            {
                _outline.SetOutline(_gridMinMax.MinMax, position, radius / 2f, mode);
            }
        }
    }
}