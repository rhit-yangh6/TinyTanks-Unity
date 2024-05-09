using UnityEngine;

namespace TerraformingTerrain2d
{
    public struct MarchingSquareTerraformingJobData
    {
        public readonly MarchingSquareFunctionsList SquareFunctionsList;
        public NativeMarchingSquareConfigurationsPresenter Configurations;
        public readonly ChunkBounds Bounds;
        public readonly Vector2Int Density;
        public readonly float IsoValue;

        public MarchingSquareTerraformingJobData(ChunkBounds bounds, Vector2Int density, float isoValue,
            NativeMarchingSquareConfigurationsPresenter configurations)
        {
            SquareFunctionsList = new MarchingSquareFunctionsList();
            Configurations = configurations;
            IsoValue = isoValue;
            Density = density;
            Bounds = bounds;
        }
    }
}