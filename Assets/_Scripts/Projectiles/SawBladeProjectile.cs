using System;
using System.Linq;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class SawBladeProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject sawBladeSmallPrefab;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private ParticleSystem sparks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _damageInterval, _moveTime, _sawBladeSmallDamage;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 3 ? _damage * 1.3f : _damage;
        protected override float MaxMagnitude => Level >= 4 ? _maxMagnitude * 1.2f : _maxMagnitude;
        protected override int Steps => Level >= 4 ? (int)(_steps * 1.3f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private int _moveDirection;
        private bool _isActivated;
        private float _intervalTimeLeft;
        private float _timeLeft;

        private void Start()
        {
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

            if (Level >= 2 && enumerable.Any(e => ReferenceEquals(Shooter, e.gameObject)))
            {
                Destroy(gameObject);
                return;
            }
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);

            if (Level == 6)
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
            
            _intervalTimeLeft = _damageInterval;
            if (Level == 5)
            {
                RefreshTimeLeft();
            }
        }

        public void SpawnPieces()
        {
            // Spawn three smaller ones
            var pos = transform.position;
            // First Piece
            var derivedObject = Instantiate(sawBladeSmallPrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<SawBladeSmallProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
        
            derivedProjectile.SetParameters(_sawBladeSmallDamage, Radius, ExplosionDuration, ExplosionFX);
            derivedProjectile.SetOtherParameters(_damageInterval, _moveTime);
            derivedProjectile.Shooter = Shooter;
            derivedRb2d.velocity = (Vector2.left + Vector2.up * 2) * 3f;
            
            // Second Piece
            derivedObject = Instantiate(sawBladeSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<SawBladeSmallProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
        
            derivedProjectile.SetParameters(_sawBladeSmallDamage, Radius, ExplosionDuration, ExplosionFX);
            derivedProjectile.SetOtherParameters(_damageInterval, _moveTime);
            derivedProjectile.Shooter = Shooter;
            derivedRb2d.velocity = (Vector2.right + Vector2.up * 2) * 3f;
            
            // Third Piece - 50/50 Left/Right
            derivedObject = Instantiate(sawBladeSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<SawBladeSmallProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
        
            derivedProjectile.SetParameters(_sawBladeSmallDamage, Radius, ExplosionDuration, ExplosionFX);
            derivedProjectile.SetOtherParameters(_damageInterval, _moveTime);
            derivedProjectile.Shooter = Shooter;
            derivedRb2d.velocity = (Random.value > 0.5 ? Vector2.right : Vector2.left + Vector2.up * 3) * 3f;
        }

        public override void Detonate()
        {
            if (_isActivated) return;
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
        }

        public override void Activate()
        {
            _moveDirection = Rigidbody2D.velocity.x > 0 ? 1 : -1;
            RefreshTimeLeft();
        }

        private void RefreshTimeLeft()
        {
            _timeLeft = Level >= 3 ? _moveTime * 1.8f : _moveTime;
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
            _sawBladeSmallDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "sawBladeSmallDamage").value;
        }
    }
}