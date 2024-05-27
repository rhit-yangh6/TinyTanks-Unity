using System;
using _Scripts.Buffs;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CurseBombProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => Level >= 4 ? _radius * 1.4f : _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => Level >= 3 ? _maxMagnitude * 1.4f : _maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(_steps * 1.4f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        private int FinalBuffLevel
        {
            get
            {
                return Level switch
                {
                    5 => 2,
                    6 => 3,
                    _ => 1
                };
            }
        }

        private void Update()
        {
            Spin();
        }
        
        public override void DealDamage()
        {
            var pos = transform.position;

            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                false, GameAssets.i.cursedBuff, FinalBuffLevel);
        }

        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.curseFX;
        }
    }
}