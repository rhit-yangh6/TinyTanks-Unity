using System;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ElectricOrbProjectile: LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        // private static float _electricChainRadius;
        
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
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            transform.Rotate(0, 0, velocity.x > 0 ? -1 : 1);
        }

        public override void Detonate()
        {
            var pos = transform.position;
            
            // DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            SpawnExplosionFX();
            DoCameraShake();
            
            var closestEntity = DamageHandler.i.DetectNearestTarget(pos, Radius, null);
            Vector2 previousPosition = default;

            if (closestEntity == null)
            {
                Destroy(gameObject);
                return;
            }

            // First chain, won't deal damage if there's no entity in the radius
            var electricLine = Instantiate(GameAssets.i.electricLineFX).GetComponent<LineController>();
            var entityPos = closestEntity.transform.position;
            electricLine.AssignPositions(pos, entityPos);
            previousPosition = entityPos;
            closestEntity.TakeDamage(Damage);
            
            // Second chain, comes with weapon basic level
            closestEntity = DamageHandler.i.DetectNearestTarget(previousPosition, Radius, closestEntity);
            
            if (closestEntity == null)
            {
                Destroy(gameObject);
                return;
            }
            electricLine = Instantiate(GameAssets.i.electricLineFX).GetComponent<LineController>();
            entityPos = closestEntity.transform.position;
            electricLine.AssignPositions(previousPosition, entityPos);
            closestEntity.TakeDamage(Damage);
            Destroy(gameObject);
        }
        
        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.chargeFX;

            // _electricChainRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "electricChainRadius").value;
        }
    }
}