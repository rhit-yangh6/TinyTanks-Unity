using System;
using System.Linq;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SawBladeSmallProjectile : DerivedProjectile
    {
        // Set in Inspector
        [SerializeField] private ParticleSystem sparks;
        
        // Other Variables
        private int _moveDirection;
        private bool _isActivated;
        private float _intervalTimeLeft;
        private float _timeLeft;
        private float _moveTime;
        private float _damageInterval;

        protected override void Start()
        {
            base.Start();
            sparks.Stop();
        }

        private void Update()
        {
            Spin();
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
            defaultMmFeedbacks.PlayFeedbacks();
        }

        public override void Activate()
        {
            _moveDirection = rigidBody2D.velocity.x > 0 ? 1 : -1;
            _timeLeft = _moveTime;
        }

        public void SetOtherParameters(float damageInterval, float moveTime)
        {
            _damageInterval = damageInterval;
            _moveTime = moveTime;
            _intervalTimeLeft = _damageInterval;
        }
    }
}