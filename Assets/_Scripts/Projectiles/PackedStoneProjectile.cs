using System;
using _Scripts.Managers;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class PackedStoneProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject packedStoneSmallPrefab;
        [SerializeField] private float packedStoneSmallRadius = 2.0f;
        [SerializeField] private float packedStoneSmallDamage = 15.0f;
        [SerializeField] private float smallProjectileSpread = 12f;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;

        private bool _isActivated;
        private void Update()
        {
            Spin(1.2f);
            if (!Input.GetMouseButtonDown(0) || _isActivated) return;
            
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
        }

        public override void Activate()
        {
            var pos = transform.position;
            var currentVelocity = rigidBody2D.velocity;
            // First Piece
            var derivedObject = Instantiate(packedStoneSmallPrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(packedStoneSmallDamage, packedStoneSmallRadius);
            derivedRb2d.velocity = Geometry.Rotate(currentVelocity, smallProjectileSpread);
            
            // Second Piece
            derivedObject = Instantiate(packedStoneSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(packedStoneSmallDamage, packedStoneSmallRadius);
            derivedRb2d.velocity = currentVelocity;
            
            // Third Piece
            derivedObject = Instantiate(packedStoneSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(packedStoneSmallDamage, packedStoneSmallRadius);
            derivedRb2d.velocity = Geometry.Rotate(currentVelocity, -smallProjectileSpread);
        }
    }
}
