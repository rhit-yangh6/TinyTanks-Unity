using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class SpikyBallProjectile: LaunchedProjectile
    {
         // Set in Inspector
        [SerializeField] private GameObject ballPrefab, spikePrefab;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _spikeDamage, _spikeRadius, _ballDamage;
        
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
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            transform.Rotate (0,0, velocity.x > 0 ? -1 : 1);
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                
                var derivedObject = Instantiate(ballPrefab, transform.position, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            
                derivedProjectile.SetParameters(_ballDamage, Radius, ExplosionDuration, ExplosionFX);
                
                
                
                Destroy(gameObject);
            }
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

            _spikeDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "spikeDamage").value;
            _spikeRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "spikeRadius").value;
            _ballDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "ballDamage").value;
        }
    }
}