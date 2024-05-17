using System;
using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ShotgunProjectile : LaunchedProjectile
    {
        // Set In Inspector        
        [SerializeField] private GameObject shotgunSecondaryPrefab;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // Extra Fields
        private static float _bulletDispersion;
        
        // References
        protected override float Radius => Level == 6 ? _radius * 1.5f : _radius;
        protected override float Damage => Level >= 3 ? _damage * 1.25f : _damage;
        protected override float MaxMagnitude => Level >= 4 ? _maxMagnitude * 1.3f : _maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(_steps * 1.3) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        private float BulletDispersion
        {
            get
            {
                return Level switch
                {
                    6 => _bulletDispersion * 0.5f,
                    >= 4 => _bulletDispersion * 0.7f,
                    _ => _bulletDispersion
                };
            }
        }

        // Other Variables
        private Rigidbody2D _rb;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            var velocity = _rb.velocity;
            var pos = transform.position;
            
            // Make clones
            var derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(velocity, BulletDispersion);
            
            derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(velocity, -BulletDispersion);

            if (Level != 5) return;
            derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(velocity, 2*BulletDispersion);
                
            derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();

            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(velocity, -2*BulletDispersion);

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
            
            _bulletDispersion = Array.Find(extraWeaponTerms, ewt => ewt.term == "bulletDispersion").value;
        }
        
        
        
    }
}
