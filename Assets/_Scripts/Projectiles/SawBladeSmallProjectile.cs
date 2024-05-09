using System;
using System.Linq;
using _Scripts.Managers;
using TerraformingTerrain2d;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SawBladeSmallProjectile : DerivedProjectile
    {
        // Shared Fields
        private static float _radius, _damage,_explosionDuration;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private int _moveDirection;
        private bool _isActivated;
        private float _intervalTimeLeft;
        private float _timeLeft;
        private float _moveTime;
        private float _damageInterval;
        
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
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
                transform.position = new Vector3(transform.position.x+_moveDirection * 7.5f * Time.deltaTime,
                    transform.position.y, transform.position.z);
            }
            
            if (!_isActivated) return;
            
            if (_timeLeft <= 0)
            {
                Destroy(gameObject);
            }
            
            _timeLeft -= Time.deltaTime;
            
            var pos = transform.position;
            var targets = DamageHandler.i.DetectTargets(pos, Radius);
            
            var enumerable = targets.ToList();
            if (_intervalTimeLeft > 0 || !enumerable.Any()) return;

            if (enumerable.Any(e => ReferenceEquals(Shooter, e.gameObject)))
            {
                Destroy(gameObject);
                return;
            }
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            _intervalTimeLeft = _damageInterval;
        }
        
        public override void Detonate()
        {
            if (_isActivated) return;
            _isActivated = true;
            _moveDirection = _rb.velocity.x > 0 ? 1 : -1;
            _timeLeft = _moveTime;
        }

        public void SetOtherParameters(float damageInterval, float moveTime)
        {
            _damageInterval = damageInterval;
            _moveTime = moveTime;
            _intervalTimeLeft = _damageInterval;
        }

        public override void SetParameters(float damage, float radius, float explosionDuration, GameObject explosionFX)
        {
            _radius = radius;
            _damage = damage;
            _explosionDuration = explosionDuration;
            _explosionFX = explosionFX;
        }
    }
}