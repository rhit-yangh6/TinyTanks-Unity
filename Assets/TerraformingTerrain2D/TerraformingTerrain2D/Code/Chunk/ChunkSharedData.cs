using UnityEngine;

namespace TerraformingTerrain2d
{
    public class ChunkSharedData
    {
        public readonly TerraformingPresenter TerraformingPresenter;
        public readonly MarchingSquareInput Input;
        public readonly bool UseMultiThreading;
        public readonly GridData GridData;
        public readonly Material Material;
        public readonly Transform Parent;
        public readonly int SortingOrder;

        public ChunkSharedData(GridData gridData, MarchingSquareInput input, Material material, Transform parent,
            bool useMultiThreading, TerraformingPresenter terraformingPresenter, int sortingOrder)
        {
            TerraformingPresenter = terraformingPresenter;
            UseMultiThreading = useMultiThreading;
            SortingOrder = sortingOrder;
            Material = material;
            GridData = gridData;
            Parent = parent;
            Input = input;
        }
    }
}