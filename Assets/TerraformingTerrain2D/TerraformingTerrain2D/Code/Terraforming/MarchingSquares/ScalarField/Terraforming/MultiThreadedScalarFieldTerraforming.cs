using Unity.Jobs;

namespace TerraformingTerrain2d
{
    public class MultiThreadedScalarFieldTerraforming : ScalarFieldTerraforming
    {
        public MultiThreadedScalarFieldTerraforming(GridData gridData, IsoValue isoValue, ScalarFieldValue value) :
            base(gridData, isoValue, value)
        {
        }

        protected override void ProceedJob(ref ScalarFieldTerraformingJob terraformingJob, int iterations)
        {
            terraformingJob.Schedule(iterations, 32).Complete();
        }
    }
}