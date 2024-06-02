using System;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class AirstrikeMissileProjectile : DerivedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject smallMissilePrefab;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private float splitThreshold = 2f;
        [SerializeField] private float smallMissileDamage = 12f;
        [SerializeField] private float smallMissileRadius = 2f;
        
        // Shared Fields
        private static float _radius, _damage,_explosionDuration;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other variables
        private Vector2 targetPos;
        private bool isSplitting;
        private bool isActivated;

        private void Start()
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }

        private void Update()
        {
            Direct();
            
            if (isSplitting && !isActivated)
            {
                var pos = transform.position;
                if (Math.Abs(pos.y - targetPos.y) > splitThreshold)
                {
                    return;
                }
                
                isActivated = true;
                activateMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            // Spawn four smaller projectiles
            var pos = transform.position;
            // First Piece
            var derivedObject = Instantiate(smallMissilePrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(smallMissileDamage, smallMissileRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(Rigidbody2D.velocity, -20);
            
            derivedObject = Instantiate(smallMissilePrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(smallMissileDamage, smallMissileRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(Rigidbody2D.velocity, -5);
            
            derivedObject = Instantiate(smallMissilePrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(smallMissileDamage, smallMissileRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(Rigidbody2D.velocity, 5);
            
            derivedObject = Instantiate(smallMissilePrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(smallMissileDamage, smallMissileRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(Rigidbody2D.velocity, 20);
        }

        public void SetTargetPos(Vector2 pos)
        {
            targetPos = pos;
        }

        public void SetIsSplitting()
        {
            isSplitting = true;
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