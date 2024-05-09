using System;
using _Scripts.Buffs;
using _Scripts.Managers;
using TerraformingTerrain2d;
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

        // ExtraFields
        // private static float _boulderPieceRadius, _boulderPieceDamage;
        
        // References
        protected override float Radius => Level >= 2 ? _radius * 1.3f : _radius;
        protected override float Damage => Level >= 3 ? _damage * 1.35f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public override void Detonate()
        {
            Vector2 pos = transform.position;

            var isCompleteHeal = false;
            if (Level >= 4) isCompleteHeal = Random.value > 0.95;
            if (Level == 5) isCompleteHeal = Random.value > 0.75;

            if (isCompleteHeal)
            {
                DamageHandler.i.HandleCompleteHeals(pos, Radius, Level == 6 ? GameAssets.i.healingBuff : null);
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                    false, Level == 6 ? GameAssets.i.healingBuff : null);
            }

            SpawnExplosionFX();
        
            Destroy(gameObject);
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            transform.Rotate (0,0, velocity.x > 0 ? -1 : 1);
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

            // _boulderPieceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceDamage").value;
            // _boulderPieceRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceRadius").value;
        }
    }
}