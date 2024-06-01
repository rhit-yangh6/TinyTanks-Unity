using System;
using System.Collections;
using _Scripts.Managers;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class AirstrikeEnhancedProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float missileDeviateAngle = 3f;
        [SerializeField] private float carpetBombingMultiplier = 0.7f;
        [SerializeField] private float missilePosDeviation = 1f;
        [SerializeField] private MMFeedbacks carpetBombingMmFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        // Other Variables
        private const float XOffset = -10f;
        private const float YOffset = -2f;
        private const float MissileSpeed = 15f;

        public override void Detonate()
        {
            if (isDetonated)
            {
                return;
            }
            isDetonated = true;
            Disappear();

            if (Level == 5)
            {
                carpetBombingMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public void SpawnMissile()
        {
            Vector2 pos = transform.position;
            
            // RayCast to sky
            var hit = Physics2D.Raycast(pos, Vector2.up, 1000, layerMask);
            
            // Calculate missile spawn point
            var missilePos = new Vector2(hit.point.x + XOffset, hit.point.y + YOffset);
            
            // Calculate missile Velocity
            var missileVelocity = Geometry.Rotate((pos - missilePos).normalized,
                Random.Range(-missileDeviateAngle, missileDeviateAngle)) * MissileSpeed;
            
            // Instantiate Missile
            var derivedObject = Instantiate(missilePrefab, missilePos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = missileVelocity;
        }

        public void SpawnCarpetBombingMissile()
        {
            Vector2 pos = transform.position;
            
            // RayCast to sky
            var hit = Physics2D.Raycast(pos, Vector2.up, 1000, layerMask);
            
            // Calculate missile spawn point
            var missilePos = new Vector2(hit.point.x + XOffset +
                                         Random.Range(-missilePosDeviation, missilePosDeviation),
                hit.point.y + YOffset);
            
            // Calculate missile Velocity
            var missileVelocity = Geometry.Rotate((pos - missilePos).normalized,
                Random.Range(-missileDeviateAngle, missileDeviateAngle)) * MissileSpeed;
            
            // Instantiate Missile
            var derivedObject = Instantiate(missilePrefab, missilePos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage * carpetBombingMultiplier,
                Radius * carpetBombingMultiplier, ExplosionDuration, ExplosionFX);
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