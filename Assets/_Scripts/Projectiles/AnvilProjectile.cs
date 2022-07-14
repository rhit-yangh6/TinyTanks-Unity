using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class AnvilProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _gravityScaleMultiplier, _fallDamageMultiplier;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated;
        private Rigidbody2D _rb;
        
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (!_isActivated)
            {
                Vector2 velocity = _rb.velocity;
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                _rb.velocity = Vector2.zero;
                _rb.gravityScale *= _gravityScaleMultiplier;
                transform.rotation = Quaternion.identity;
            }
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else
            {
                Detonate();
            }
        }
        
        public override void Detonate()
        {
            Vector2 pos = transform.position;

            float damageDealt = _isActivated ? _damage * _fallDamageMultiplier : _damage;
            
            DamageHandler.i.HandleCircularDamage(pos, _radius, damageDealt);
            
            TerrainDestroyer.Instance.DestroyTerrain(pos, _radius);
        
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

            _gravityScaleMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "gravityScaleMultiplier").value;
            _fallDamageMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "fallDamageMultiplier").value;
        }
    }
}
