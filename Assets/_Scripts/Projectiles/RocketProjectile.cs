using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class RocketProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _velocityMultiplier;
        
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
        private ParticleSystem _ps;
    
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _ps = gameObject.GetComponentInChildren<ParticleSystem>();
            _ps.Stop();
        }

        private void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                velocity *= _velocityMultiplier;
                _rb.velocity = velocity;
                _ps.Play();
            }
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.regularExplosionFX;
  
            _velocityMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "velocityMultiplier").value;
        }
    }
}
