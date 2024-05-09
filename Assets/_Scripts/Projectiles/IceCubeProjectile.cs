using System;
using _Scripts.Buffs;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class IceCubeProjectile: LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        // private static float _gravityScaleMultiplier, _fallDamageMultiplier, _secondPhaseFallDamageMultiplier;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 2 ? _damage * 1.1f : _damage;
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
            
            var finalBuffLevel = Level switch
            {
                5 => 4,
                6 => 5,
                4 => 3,
                3 => 2,
                _ => 1
            };
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular, 
                false, GameAssets.i.frozenBuff, finalBuffLevel);

            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;

            //_gravityScaleMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "gravityScaleMultiplier").value;
        }
    }
}