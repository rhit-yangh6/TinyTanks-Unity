using _Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class MedKitProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => Level >= 2 ? _radius * 1.3f : _radius;
        protected override float Damage => Level >= 3 ? _damage * 1.35f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        private bool IsCompleteHeal
        {
            get
            {
                return Level switch
                {
                    5 => Random.value > 0.75,
                    >= 4 => Random.value > 0.95,
                    _ => false
                };
            }
        }

        public override void DealDamage()
        {
            Vector2 pos = transform.position;

            if (IsCompleteHeal)
            {
                DamageHandler.i.HandleCompleteHeals(pos, Radius, Level == 6 ? GameAssets.i.healingBuff : null);
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                    false, Level == 6 ? GameAssets.i.healingBuff : null);
            }
        }

        private void Update()
        {
            Spin();
        }

        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.healFX;
        }
    }
}