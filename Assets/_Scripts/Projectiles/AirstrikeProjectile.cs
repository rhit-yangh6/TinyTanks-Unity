using System;
using System.Collections;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
     public class AirstrikeProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private LayerMask layerMask;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        // private static float _detonateTime, _clusterExplosionRadius, _clusterExplosionDamage;
        // private static float _secondaryExplosionRadius, _secondaryExplosionDamage;

        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        // Other Variables
        private SpriteRenderer _sr;
        private Renderer _r;
        private Rigidbody2D _rb;
        private GameObject _target;
        private bool _isActivated;
        private const float RotationAmount = 0.9f;
        private const float ResizeAmount = 0.008f;
        private const float DelaySpeed = .01f;
        private const float XOffset = -10f;
        private const float YOffset = -2f;
        private const float MissileSpeed = 15f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
            _sr = GetComponent<SpriteRenderer>();
        }

        private IEnumerator RotateTarget()
        {
            float count = 0;
            while(count <= 3){
                _target.transform.Rotate(new Vector3(0, 0, RotationAmount));
                count += DelaySpeed;
                yield return new WaitForSeconds(DelaySpeed);
            }
            Destroy(_target);
            Destroy(gameObject);
        }
        
        private IEnumerator ResizeTarget()
        {
            float count = 0;
            while(count <= 1)
            {
                _target.transform.localScale += new Vector3(ResizeAmount, ResizeAmount, 0);
                count += DelaySpeed;
                yield return new WaitForSeconds(DelaySpeed);
            }

            count = 0;
            while(count <= 2)
            {
                _target.transform.localScale += new Vector3(-ResizeAmount, -ResizeAmount, 0);
                count += DelaySpeed;
                yield return new WaitForSeconds(DelaySpeed);
            }
        }

        public override void Detonate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;
            Vector2 pos = transform.position;

            _rb.velocity = Vector3.zero;
            _rb.gravityScale = 0;
            _r.enabled = false;
            
            // Instantiate a target
            _target = Instantiate(GameAssets.i.targetFX, pos, Quaternion.identity);
            StartCoroutine(RotateTarget());
            StartCoroutine(ResizeTarget());
            
            // RayCast to sky
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, 1000, layerMask);
            
            // Calculate missile spawn point
            Vector2 missilePos = new Vector2(hit.point.x + XOffset, hit.point.y + YOffset);
            
            // Calculate missile Velocity
            Vector2 missileVelocity = (pos - missilePos).normalized * MissileSpeed;
            
            // Instantiate Missile
            var derivedObject = Instantiate(missilePrefab, missilePos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = missileVelocity;
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude,
            int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.regularExplosionFX;
        }
    }
}