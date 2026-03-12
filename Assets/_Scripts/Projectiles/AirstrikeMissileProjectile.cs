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
        
        // Other variables
        private Vector2 targetPos;
        private bool isSplitting;
        private bool isActivated;

        protected override void Start()
        {
            base.Start();
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
            Vector2 pos = transform.position;
            var velocity = rigidBody2D.linearVelocity;

            foreach (var angle in new[] { -20f, -5f, 5f, 20f })
            {
                SpawnDerived(smallMissilePrefab, pos, smallMissileDamage, smallMissileRadius,
                    Geometry.Rotate(velocity, angle));
            }
        }

        public void SetTargetPos(Vector2 pos)
        {
            targetPos = pos;
        }

        public void SetIsSplitting()
        {
            isSplitting = true;
        }
    }
}