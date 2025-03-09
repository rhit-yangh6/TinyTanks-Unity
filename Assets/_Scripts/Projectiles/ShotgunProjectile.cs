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
        [SerializeField] private float bulletDispersion = 10.0f;
        
        // References
        protected override float Radius => Level == 6 ? radius * 1.5f : radius;
        protected override float Damage => Level >= 3 ? damage * 1.25f : damage;
        protected override float MaxMagnitude => Level >= 4 ? maxMagnitude * 1.3f : maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(steps * 1.3) : steps;
        private float BulletDispersion
        {
            get
            {
                return Level switch
                {
                    6 => bulletDispersion * 0.5f,
                    >= 4 => bulletDispersion * 0.7f,
                    _ => bulletDispersion
                };
            }
        }
        
        protected override void Start()
        {
            base.Start();
            SpawnSecondaryProjectiles();
        }

        private void SpawnSecondaryProjectiles()
        {
            var velocity = rigidBody2D.velocity;
            var pos = transform.position;
            
            // Make clones
            var derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = Geometry.Rotate(velocity, BulletDispersion);
            
            derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = Geometry.Rotate(velocity, -BulletDispersion);

            if (Level != 5) return;
            derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = Geometry.Rotate(velocity, 2*BulletDispersion);
                
            derivedObject = Instantiate(shotgunSecondaryPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();

            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = Geometry.Rotate(velocity, -2*BulletDispersion);
        }
    }
}
