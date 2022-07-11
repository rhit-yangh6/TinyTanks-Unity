namespace _Scripts.Projectiles
{
    public interface IProjectile
    {

        int Level { get; set; }

        void Detonate();

        void SpawnExplosionFX();

        void DoCameraShake();
        
        void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms);

        float GetMaxMagnitude();

        int GetSteps();

        float GetFixedMagnitude();
        
    }
}
