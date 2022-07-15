using UnityEngine;

namespace _Scripts.Projectiles
{
    public class DerivedProjectile : MonoBehaviour, IProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected virtual float Radius => _radius;
        protected virtual float Damage => _damage;
        protected virtual float ExplosionDuration => _explosionDuration;
        protected virtual GameObject ExplosionFX => _explosionFX;
        
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
            Camera.main.GetComponent<CameraMovement>().ShakeCamera(ExplosionDuration);
        }

        public virtual void SetParameters(float damage, float radius, float explosionDuration, GameObject explosionFX) { }
    }
}