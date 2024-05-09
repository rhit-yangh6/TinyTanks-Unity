
namespace TerraformingTerrain2d
{
    public class MeshConfigurationPresenter
    {
        private readonly MarchingSquareMeshConfiguration[] _meshEvaluations;

        public MeshConfigurationPresenter(MarchingSquareInput input)
        {
            _meshEvaluations = new MarchingSquareMeshConfiguration[]
            {
                new SquareMeshConfiguration0(input),
                new SquareMeshConfiguration1(input),
                new SquareMeshConfiguration2(input),
                new SquareMeshConfiguration3(input),
                new SquareMeshConfiguration4(input),
                new SquareMeshConfiguration5(input),
                new SquareMeshConfiguration6(input),
                new SquareMeshConfiguration7(input),
                new SquareMeshConfiguration8(input),
                new SquareMeshConfiguration9(input),
                new SquareMeshConfiguration10(input),
                new SquareMeshConfiguration11(input),
                new SquareMeshConfiguration12(input),
                new SquareMeshConfiguration13(input),
                new SquareMeshConfiguration14(input),
                new SquareMeshConfiguration15(input)
            };
        }

        public MarchingSquareMeshConfiguration GetConfiguration(int configuration)
        {
            return _meshEvaluations[configuration];
        }
    }
}