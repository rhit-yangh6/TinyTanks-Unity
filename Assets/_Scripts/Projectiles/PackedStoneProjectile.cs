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
        [SerializeField] private MMFeedbacks activateMmFeedbacks;

        protected override float Damage => Level >= 2 ? damage * 1.15f : damage;

        private float PackedStoneSmallDamage
        {
            get
            {
                return Level switch
                {
                    6 => packedStoneSmallDamage * 0.7f,
                    5 => packedStoneSmallDamage * 1.25f,
                    >= 2 => packedStoneSmallDamage * 1.15f,
                    _ => packedStoneSmallDamage
                };
            }
        }
        
        private float PackedStoneSmallRadius
        {
            get
            {
                return Level switch
                {
                    6 => packedStoneSmallRadius * 0.5f,
                    >= 3 => packedStoneSmallRadius * 1.3f,
                    _ => packedStoneSmallRadius
                };
            }
        }

        private float SmallProjectileSpread
        {
            get
            {
                return Level switch
                {
                    5 => 30f,
                    _ => 12f
                };
            }
        }
        
        
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
            switch (Level)
            {
                case 6:
                    SpawnSmallStones(7);
                    break;
                case >= 4:
                    SpawnSmallStones(4);
                    break;
                default:
                    SpawnSmallStones(3);
                    break;
            }
        }
        
        private void SpawnSmallStones(int amount)
        {
            var pos = transform.position;
            var currentVelocity = rigidBody2D.velocity;
            var currentAngle = -SmallProjectileSpread;
            var angleStep = (SmallProjectileSpread * 2) / (amount - 1);

            for (var i = 0; i < amount; i++)
            {
                var derivedObject = Instantiate(packedStoneSmallPrefab, pos, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(PackedStoneSmallDamage, PackedStoneSmallRadius);
                derivedRb2d.velocity = Geometry.Rotate(currentVelocity, currentAngle);
                currentAngle += angleStep;
            }
        }
    }
}
