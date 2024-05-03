using System;
using System.Collections;
using _Scripts.Entities;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SacrificialBondProjectile: LaunchedProjectile
    {
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _initialGrowDuration, _enemyDamageMultiplier;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();

            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;
            _sr.enabled = false;

            StartCoroutine(StartSacrifice());
        }
        
        private IEnumerator StartSacrifice()
        {
            var fxPrefab= Instantiate(ExplosionFX, gameObject.transform.position, Quaternion.identity);
            
            yield return new WaitForSeconds(_initialGrowDuration);
            DoCameraShake();
            
            // Deal Damage
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<BuffableEntity>().TakeDamage(Damage * _enemyDamageMultiplier);
            }
            // Sacrifice self, it is always critical hit
            Shooter.GetComponent<BuffableEntity>().TakeDamage(Damage, true);
            
            // Wait until explosion ended and destroy the gameObject
            yield return new WaitForSeconds(ExplosionDuration);
            Destroy(fxPrefab);
            Destroy(gameObject);
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

            _initialGrowDuration = Array.Find(extraWeaponTerms, ewt => ewt.term == "initialGrowDuration").value;
            _enemyDamageMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "enemyDamageMultiplier").value;
        }
    }
}