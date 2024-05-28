using _Scripts.Managers;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CoconutProjectile: DerivedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks icedMmFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage,_explosionDuration;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Extra Fields
        [HideInInspector] public bool isIced;
        
        public override void Detonate()
        {
            if (IsDetonated) return;
            IsDetonated = true;

            Disappear();
            DealDamage();

            if (isIced)
            {
                icedMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void DealDamage()
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