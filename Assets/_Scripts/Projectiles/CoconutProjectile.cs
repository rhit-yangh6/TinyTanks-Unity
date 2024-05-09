using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CoconutProjectile: DerivedProjectile
    {
        // Shared Fields
        private static float _radius, _damage,_explosionDuration;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Extra Fields
        public bool isIced;

        public override void Detonate()
        {
            Vector2 pos = transform.position;
            if (isIced)
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular, 
                    false, GameAssets.i.frozenBuff, 2);
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            }
            
            SpawnExplosionFX();
            DoCameraShake();
            
            Destroy(gameObject);
        }
        
        public override void SetParameters(float damage, float radius, float explosionDuration, GameObject explosionFX)
        {
            _radius = radius;
            _damage = damage;
            _explosionDuration = explosionDuration;
            _explosionFX = explosionFX;
        }
    }
}