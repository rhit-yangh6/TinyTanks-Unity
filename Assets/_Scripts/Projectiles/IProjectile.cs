using TerraformingTerrain2d;

namespace _Scripts.Projectiles
{
    public interface IProjectile
    {
        void Detonate();

        void SpawnExplosionFX();

        void DoCameraShake();

    }
}
