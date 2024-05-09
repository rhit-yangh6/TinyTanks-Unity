
namespace TerraformingTerrain2d
{
    public readonly struct TerraformingJobResult
    {
        public readonly int PhysicsShapesCount;
        public readonly int TrianglesCount;

        public TerraformingJobResult(int trianglesCount, int physicsShapesCount)
        {
            PhysicsShapesCount = physicsShapesCount;
            TrianglesCount = trianglesCount;
        }
    }
}