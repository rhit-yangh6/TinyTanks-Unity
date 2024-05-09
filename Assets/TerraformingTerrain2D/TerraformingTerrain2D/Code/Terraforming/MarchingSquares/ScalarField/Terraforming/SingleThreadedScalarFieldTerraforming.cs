
namespace TerraformingTerrain2d
{
    public class SingleThreadedScalarFieldTerraforming : ScalarFieldTerraforming
    {
        public SingleThreadedScalarFieldTerraforming(GridData gridData, IsoValue isoValue, ScalarFieldValue value) :
            base(gridData, isoValue, value)
        {
        }

        protected override void ProceedJob(ref ScalarFieldTerraformingJob terraformingJob, int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                terraformingJob.Execute(i);
            }
        }
    }
}