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
        
        // References
        protected override float Damage {
            get
            {
                if (Level >= 1) return damage * 1.38f;
                return Level >= 3 ? damage * 1.15f : damage;
            }
        }
        protected override int Steps => steps;
        
        // Other Variables
        private bool _isActivated;
    
        private void Start()
        {
        }

        private void Update()
        {
            Direct();
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
            derivedRb2d.velocity = (Vector2.left + Vector2.up * 2) * 3f;
            
            // Second Piece
            derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = (Vector2.right + Vector2.up * 2) * 3f;
        }
    }
}
