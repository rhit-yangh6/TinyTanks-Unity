using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SawBladeProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _damageInterval, _moveTime;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private int _moveDirection;
        private bool _isActivated;
        private float _intervalTimeLeft;

        // TODO; refresh when hitting targets
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            transform.Rotate(0, 0, _rb.velocity.x > 0 ? -1 : 1);
            if (_intervalTimeLeft > 0)
            {
                _intervalTimeLeft -= Time.deltaTime;
            }

            if (_moveDirection != 0)
            {
                transform.position = new Vector3(transform.position.x+_moveDirection * 7f * Time.deltaTime,
                    transform.position.y, transform.position.z);
            }

            if (!_isActivated) return;

            var pos = transform.position;
            var hasTarget = DamageHandler.i.DetectTargets(pos, Radius);

            if (_intervalTimeLeft > 0 || !hasTarget) return;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            _intervalTimeLeft = _damageInterval;
        }

        public override void Detonate()
        {
            if (_isActivated) return;
            _isActivated = true;
            _moveDirection = _rb.velocity.x > 0 ? 1 : -1;
            StartCoroutine(MoveForward());
        }

        private IEnumerator MoveForward()
        {
            yield return new WaitForSeconds(_moveTime);
            _moveDirection = 0;
            
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

            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
            
            _damageInterval = Array.Find(extraWeaponTerms, ewt => ewt.term == "damageInterval").value;
            _moveTime = Array.Find(extraWeaponTerms, ewt => ewt.term == "moveTime").value;
        }
    }
}