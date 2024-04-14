using System;
using System.Collections;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CoconutSeedProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject coconutTreePrefab, coconutProjectilePrefab;
        
         // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _coconutDamage, _coconutRadius;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private Renderer _r;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
        }

        public override void Detonate()
        {
            Vector2 pos = transform.position;
            
            // DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;

            _r.enabled = false;
            
            StartCoroutine(GrowCoconutTree(pos));
        }
        
        private IEnumerator GrowCoconutTree(Vector2 pos)
        {
            var coconutTree = 
                Instantiate(coconutTreePrefab, new Vector2(pos.x, pos.y - 1.5f), Quaternion.identity);

            yield return new WaitForSeconds(2.2f);
            
            // First Coconut
            var derivedObject = Instantiate(coconutProjectilePrefab, 
                new Vector2(pos.x - 1.0f, pos.y + 3.1f), 
                Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedProjectile.SetParameters(_coconutDamage, _coconutRadius, ExplosionDuration, ExplosionFX);

            yield return new WaitForSeconds(0.15f);
            
            // Second Coconut
            derivedObject = Instantiate(coconutProjectilePrefab, 
                new Vector2(pos.x + 0.8f, pos.y + 3.3f), 
                Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedProjectile.SetParameters(_coconutDamage, _coconutRadius, ExplosionDuration, ExplosionFX);
            
            // Wait for everything finished
            // TODO: tree wait for coconuts all gone?
            yield return new WaitForSeconds(1.5f);
            
            Destroy(coconutTree);
            Destroy(gameObject);
        }

        private void Update()
        {
            transform.Rotate (0,0, _rb.velocity.x > 0 ? -1 : 1);
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

            _coconutDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "coconutDamage").value;
            _coconutRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "coconutRadius").value;
        }
    }
}