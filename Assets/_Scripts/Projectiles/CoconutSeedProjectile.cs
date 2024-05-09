using System;
using System.Collections;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CoconutSeedProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject coconutProjectilePrefab;
        [SerializeField] private Sprite coconutIceSprite;
        
         // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _coconutRadius;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 3 ? _damage * 1.2f : _damage;
        protected override float MaxMagnitude => Level >= 2 ? _maxMagnitude * 1.2f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private Renderer _r;
        private bool _isCollided;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
        }

        public override void Detonate()
        {
            if (!_isCollided)
            {
                _rb.velocity = Vector2.down;
                _isCollided = true;
                return;
            }

            _rb.isKinematic = true;
            _rb.gravityScale = 0;
            _r.enabled = false;
            _rb.velocity = Vector2.zero;
            Vector2 pos = transform.position;
            
            StartCoroutine(GrowCoconutTree(pos));
        }
        
        private IEnumerator GrowCoconutTree(Vector2 pos)
        {
            var coconutTree = 
                Instantiate(ExplosionFX, new Vector2(pos.x, pos.y - 1.5f), Quaternion.identity);

            yield return new WaitForSeconds(2.2f);
            
            // First Coconut
            var derivedObject = Instantiate(coconutProjectilePrefab, 
                new Vector2(pos.x - 1.0f, pos.y + 3.1f), 
                Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
            derivedProjectile.SetParameters(Damage, _coconutRadius, ExplosionDuration, ExplosionFX);

            if (Level == 5)
            {
                derivedObject.GetComponent<SpriteRenderer>().sprite = coconutIceSprite;
                derivedProjectile.isIced = true;
            }

            yield return new WaitForSeconds(0.15f);
            
            // Second Coconut
            derivedObject = Instantiate(coconutProjectilePrefab, 
                new Vector2(pos.x + 0.8f, pos.y + 3.3f), 
                Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
            derivedProjectile.SetParameters(Damage, _coconutRadius, ExplosionDuration, ExplosionFX);
            
            if (Level == 5)
            {
                derivedObject.GetComponent<SpriteRenderer>().sprite = coconutIceSprite;
                derivedProjectile.isIced = true;
            }
            
            yield return new WaitForSeconds(0.15f);

            // Third Coconut
            if (Level >= 4)
            {
                derivedObject = Instantiate(coconutProjectilePrefab, 
                    new Vector2(pos.x + 0.1f, pos.y + 3.2f), 
                    Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
                derivedProjectile.SetParameters(Damage, _coconutRadius, ExplosionDuration, ExplosionFX);
                
                if (Level == 5)
                {
                    derivedObject.GetComponent<SpriteRenderer>().sprite = coconutIceSprite;
                    derivedProjectile.isIced = true;
                }
            }
            
            yield return new WaitForSeconds(0.15f);
            
            // Fourth Coconut
            if (Level == 6)
            {
                derivedObject = Instantiate(coconutProjectilePrefab, 
                    new Vector2(pos.x - 0.5f, pos.y + 3.5f), 
                    Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
                derivedProjectile.SetParameters(Damage, _coconutRadius, ExplosionDuration, ExplosionFX);
            }
            
            // Wait for everything finished
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

            _explosionFX = GameAssets.i.coconutTreeFX;

            _coconutRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "coconutRadius").value;
        }
    }
}