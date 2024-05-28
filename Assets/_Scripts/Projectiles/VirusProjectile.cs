using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class VirusProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _damageInterval, _moveTime;
        
        // References
        protected override float Radius => Level >= 3 ? _radius * 1.3f : _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => Level >= 3 ? _maxMagnitude * 1.3f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        private int BuffLevel
        {
            get
            {
                return Level switch
                {
                    6 => 5,
                    5 => 4,
                    4 => 3,
                    >= 2 => 2,
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
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular, false,
                GameAssets.i.infectedBuff, BuffLevel);
        }

        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.virusExplosionFX;
        }
    }
}