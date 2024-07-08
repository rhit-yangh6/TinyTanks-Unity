using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SplitterProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private GameObject splitterSmallPrefab;
        [SerializeField] private float spawnVelocity = 3f;
        
        // References
        protected override float Damage {
            get
            {
                return Level switch
                {
                    _ => damage
                };
            }
        }
        protected override int Steps => steps;
        
        // Other Variables
        private bool _isActivated;

        private void Update()
        {
            Spin();
            if (!Input.GetMouseButtonDown(0) || _isActivated) return;
            
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
        }

        public override void Activate()
        {
            var pos = transform.position;
            // First Piece
            var derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = (Vector2.left * 2 + Vector2.up) * spawnVelocity;
            
            // Second Piece
            derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = (Vector2.right * 2 + Vector2.up) * spawnVelocity;
        }
    }
}
