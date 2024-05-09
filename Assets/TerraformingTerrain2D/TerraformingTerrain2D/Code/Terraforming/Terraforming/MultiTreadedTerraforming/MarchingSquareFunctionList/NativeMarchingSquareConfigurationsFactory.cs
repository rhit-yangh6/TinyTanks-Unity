
namespace TerraformingTerrain2d
{
    public abstract class NativeMarchingSquareConfigurationsFactory
    {
        public static NativeMarchingSquareConfigurationsPresenter Create()
        {
            return new NativeMarchingSquareConfigurationsPresenter
            {
                Value_0 = new NativeMarchingSquareMeshConfiguration(0, new int[] { }),
                Value_1 = new NativeMarchingSquareMeshConfiguration(3, new[] { 0, 1, 2 }),
                Value_2 = new NativeMarchingSquareMeshConfiguration(3, new[] { 0, 1, 2 }),
                Value_3 = new NativeMarchingSquareMeshConfiguration(4, new[] { 0, 1, 2, 0, 2, 3 }),
                Value_4 = new NativeMarchingSquareMeshConfiguration(3, new[] { 0, 1, 2 }),
                Value_5 = new NativeMarchingSquareMeshConfiguration(6, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 }),
                Value_6 = new NativeMarchingSquareMeshConfiguration(4, new[] { 0, 1, 2, 0, 2, 3 }),
                Value_7 = new NativeMarchingSquareMeshConfiguration(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, }),
                Value_8 = new NativeMarchingSquareMeshConfiguration(3, new[] { 0, 1, 2 }),
                Value_9 = new NativeMarchingSquareMeshConfiguration(4, new[] { 0, 1, 2, 0, 2, 3 }),
                Value_10 = new NativeMarchingSquareMeshConfiguration(6, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 }),
                Value_11 = new NativeMarchingSquareMeshConfiguration(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, }),
                Value_12 = new NativeMarchingSquareMeshConfiguration(4, new[] { 0, 1, 2, 0, 2, 3 }),
                Value_13 = new NativeMarchingSquareMeshConfiguration(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, }),
                Value_14 = new NativeMarchingSquareMeshConfiguration(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, }),
                Value_15 = new NativeMarchingSquareMeshConfiguration(4, new[] { 0, 1, 2, 0, 2, 3 }),
            };
        }
    }
}