using System;
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
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            var velocity = _rb.velocity;
            _rb.GetComponent<ConstantForce2D>().force = new Vector3(velocity.x * _extraForceXMultiplier,
                velocity.y * _extraForceYMultiplier, 0);
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else
            {
                _explosionFX = Level >= 3 ? GameAssets.i.regularExplosionFX : GameAssets.i.gunpowderlessExplosionFX;
                Detonate();
            }
        }
        
        public override void Detonate()
        {
            Vector2 pos = transform.position;

            var finalCalculatedRadius = Radius;
            if (Level >= 3) finalCalculatedRadius *= 1.5f;

            var isCritical = false;
            if (Level >= 4) isCritical = Random.value > 0.75;
            if (Level == 6) isCritical = true;

            DamageHandler.i.HandleDamage(pos, finalCalculatedRadius, isCritical ? Damage * 1.5f : Damage, 
                DamageHandler.DamageType.Circular, isCritical);

            if (Level >= 3) TerrainDestroyer.instance.DestroyTerrainCircular(pos, finalCalculatedRadius);

            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }
        
        public override float GetFixedMagnitude()
        {
            return _fixedMagnitude;
        }
        
        public override int GetSteps()
        {
            var finalCalculatedSteps = Steps;

            if (Level >= 2) finalCalculatedSteps = (int)(finalCalculatedSteps * 1.5f);
            // TODO: Another Level 5?
            if (Level == 5) finalCalculatedSteps = (int)(finalCalculatedSteps * 2f);
            
            return finalCalculatedSteps;
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
