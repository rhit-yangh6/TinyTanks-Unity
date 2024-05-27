using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class SniperProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _fixedMagnitude, _extraForceXMultiplier, _extraForceYMultiplier;

        // References
        protected override float Radius => Level >= 3 ? _radius * 1.5f : _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps {
            get
            {
                if (Level == 4) return (int)(_steps * 3f); // LEVEl 5
                return Level >= 2 ? (int)(_steps * 1.5f) : _steps; // LEVEL 2+
            }
        }
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        private void Start()
        {
            var velocity = Rigidbody2D.velocity;
            Rigidbody2D.GetComponent<ConstantForce2D>().force = new Vector3(velocity.x * _extraForceXMultiplier,
                velocity.y * _extraForceYMultiplier, 0);
        }

        private void Update() { Direct(); }

        public override void DealDamage()
        {
            var pos = transform.position;

            var isCritical = false;
            if (Level >= 4) isCritical = Random.value > 0.75;
            if (Level == 6) isCritical = true;

            DamageHandler.i.HandleDamage(pos, Radius, isCritical ? Damage * 1.5f : Damage, 
                DamageHandler.DamageType.Circular, isCritical);

            if (Level >= 3) EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Circular);
        }

        public override float GetFixedMagnitude()
        {
            return _fixedMagnitude;
        }

        protected override void Disappear()
        {
            base.Disappear();
            Rigidbody2D.GetComponent<ConstantForce2D>().force = Vector2.zero;
        }
        
        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;

            _fixedMagnitude = Array.Find(extraWeaponTerms, ewt => ewt.term == "fixedMagnitude").value;
            _extraForceXMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "extraForceXMultiplier").value;
            _extraForceYMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "extraForceYMultiplier").value;
        }
    }
}
