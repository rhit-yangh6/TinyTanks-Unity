using System;
using System.Collections;
using _Scripts.Entities;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SacrificialBondProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private float enemyDamage = 20f;
        [SerializeField] private float healingDamage = 15f;
        [SerializeField] private float totalDistributedDamage = 55f;
        [SerializeField] private MMFeedbacks damageAndHealMmFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    6 => _damage * 1.2f,
                    >= 2 => _damage * 0.72f,
                    _ => _damage
                };
            }
        }

        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        private float EnemyDamage
        {
            get
            {
                return Level switch
                {
                    6 => enemyDamage * 2f,
                    >= 3 => enemyDamage * 1.4f,
                    _ => enemyDamage
                };
            }
        }
        
        private void Start()
        {
            Rigidbody2D.velocity = Vector2.zero;
            Rigidbody2D.gravityScale = 0;
            renderer.enabled = false;

            if (Level >= 4)
            {                
                damageAndHealMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void DealDamage()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                if (Level == 5)
                {
                    enemy.GetComponent<BuffableEntity>().TakeDamage(totalDistributedDamage / enemies.Length);
                }
                else
                {
                    enemy.GetComponent<BuffableEntity>().TakeDamage(EnemyDamage);
                }
            }
            // Sacrifice self, it is always critical hit
            Shooter.GetComponent<BuffableEntity>().TakeDamage(Damage, true);
        }

        public void HealAfterDamage()
        { 
            Shooter.GetComponent<BuffableEntity>().TakeDamage(-healingDamage);
        }

        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.sacrificeFX;
        }
    }
}