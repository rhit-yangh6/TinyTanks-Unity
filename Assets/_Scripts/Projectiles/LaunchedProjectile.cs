using UnityEngine;

namespace _Scripts.Projectiles
{
    public class LaunchedProjectile : MonoBehaviour, IProjectile
    {
        public int Level { get; set; }

        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected virtual float Radius => _radius;
        protected virtual float Damage => _damage;
        protected virtual float MaxMagnitude => _maxMagnitude;
        protected virtual int Steps => _steps;
        protected virtual float ExplosionDuration => _explosionDuration;
        protected virtual GameObject ExplosionFX => _explosionFX;

        // The most basic detonate function
        public virtual void Detonate()
        {
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleCircularDamage(pos, Radius, Damage);

            TerrainDestroyer.Instance.DestroyTerrain(pos, Radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        
        public virtual void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(ExplosionFX, transform.position, Quaternion.identity);
            insExpl.transform.localScale *= Radius;
            Destroy(insExpl, ExplosionDuration);
        }

        public virtual void DoCameraShake()
        {
            Camera.main.GetComponent<CameraShake>().shakeDuration = ExplosionDuration;
        }

        public virtual void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms) { }

        public virtual float GetMaxMagnitude()
        {
            return MaxMagnitude;
        }

        public virtual int GetSteps()
        {
            return Steps;
        }

        public virtual float GetFixedMagnitude()
        {
            return -1f;
        }
    }
}
