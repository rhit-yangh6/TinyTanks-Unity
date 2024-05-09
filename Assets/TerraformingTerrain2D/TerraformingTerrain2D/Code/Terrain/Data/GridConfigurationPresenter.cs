using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [Serializable]
    public class GridConfigurationPresenter
    {
        [SerializeField] private GridConfiguration _preset;

        public GridConfiguration Preset => _preset;
        public bool IsInUse => _preset != null;

        public bool CheckIfValid(Vector2Int generatedChunksCount)
        {
            if (Preset.SDFTexture == null)
            {
                Debug.LogError("SDF texture is missing. Configuration is not valid");

                _preset = null;
                return false;
            }

            GridData configurationGridData = new(Preset.Density, Preset.SDFTexture.GetRatio(), 0);
            Vector2Int density = configurationGridData.SquareDensity;

            if (density.x < generatedChunksCount.x || density.y < generatedChunksCount.y)
            {
                Debug.LogError("Regenerate your chunk count with following constraint: " +
                               $"Chunk Count X <= {density.x} and " +
                               $"Chunk Count Y <= {density.y}");

                _preset = null;
                return false;
            }

            return true;
        }
    }
}