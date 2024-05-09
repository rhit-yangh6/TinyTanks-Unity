
namespace TerraformingTerrain2d
{
    public class ScalarFieldFactory
    {
        private readonly TerraformingTerrainData _data;
        private readonly GridData _gridData;

        public ScalarFieldFactory(TerraformingTerrainData data, GridData gridData)
        {
            _gridData = gridData;
            _data = data;
        }

        public ScalarField Create()
        {
            ScalarFieldValue value = new(_gridData.Density);
            ScalarFieldInitialization initialization = CreateInitialization(value);
            ScalarFieldTerraforming terraforming = CreateTerraforming(_data.UseMultiThreading, value);

            return new ScalarField(value, terraforming, initialization);
        }

        private ScalarFieldTerraforming CreateTerraforming(bool useMultiThreading, ScalarFieldValue value)
        {
            return useMultiThreading
                ? new MultiThreadedScalarFieldTerraforming(_gridData, _data.IsoValue, value)
                : new SingleThreadedScalarFieldTerraforming(_gridData, _data.IsoValue, value);
        }

        private ScalarFieldInitialization CreateInitialization(ScalarFieldValue value)
        {
            SDFTextureToTerrainConverter sdfTextureToTerrainConverter = new(_data.SdfFactor, _data.SdfTexture);
            IScalarFieldInitializer scalarFieldInitializer = _data.Configuration.IsInUse
                ? new ScalarFieldConfigurationRestore(_data.Configuration.Preset.ScalarField)
                : sdfTextureToTerrainConverter;

            return new ScalarFieldInitialization(value, _gridData.Density, scalarFieldInitializer, sdfTextureToTerrainConverter);
        }
    }
}