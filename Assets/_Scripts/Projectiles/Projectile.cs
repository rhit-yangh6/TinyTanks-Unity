namespace _Scripts.Projectiles
{
    public interface IProjectile
    {

        void Detonate();

        void SpawnExplosionFX();

        void DoCameraShake();

        void OnFinish();

        void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms);

        float getMaxMagnitude();

        int getSteps();

    }
}
