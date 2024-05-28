using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class IceCubeProjectile: LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 2 ? _damage * 1.1f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        private int BuffLevel
        {
            get
            {
                return Level switch
                {
                    5 => 4,
                    6 => 5,
                    4 => 3,
                    3 => 2,
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
                false, GameAssets.i.frozenBuff, BuffLevel);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
        }
    }
}