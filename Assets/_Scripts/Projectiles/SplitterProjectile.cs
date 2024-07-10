using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class SplitterProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private GameObject splitterSmallPrefab;
        [SerializeField] private float spawnVelocity = 3f;
        [SerializeField] private float thirdProjectileChance = 0.35f;
        [SerializeField] private float unpredictableSplitSuccessChance = 0.8f;
        
        // References
        protected override float Damage {
            get
            {
                return Level switch
                {
                    >= 2 => damage * 1.2f,
                    _ => damage
                };
            }
        }
        
        protected override float MaxMagnitude {
            get
            {
                return Level switch
                {
                    >= 3 => maxMagnitude * 1.3f,
                    _ => maxMagnitude
                };
            }
        }
        
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
            var isUnpredictableSplitSuccessful = Random.value < unpredictableSplitSuccessChance;
            if (!isUnpredictableSplitSuccessful && Level == 5)
            {
                Detonate();
                return;
            }
            var pos = transform.position;
            // First Piece
            var derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<SplitterSmallProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedProjectile.SetExtraFields(Level == 6);
            derivedRb2d.velocity = (Vector2.left * 2 + Vector2.up) * spawnVelocity;
            
            // Second Piece
            derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<SplitterSmallProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedProjectile.SetExtraFields(Level == 6);
            derivedRb2d.velocity = (Vector2.right * 2 + Vector2.up) * spawnVelocity;

            if (Level == 5 && isUnpredictableSplitSuccessful)
            {
                // Third Piece
                derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<SplitterSmallProjectile>();
                derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(Damage, Radius);
                derivedProjectile.SetExtraFields(Level == 6);
                derivedRb2d.velocity = (Vector2.up * 2 + Vector2.left) * spawnVelocity;
                    
                // Fourth Piece
                derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<SplitterSmallProjectile>();
                derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(Damage, Radius);
                derivedProjectile.SetExtraFields(Level == 6);
                derivedRb2d.velocity = (Vector2.up * 2 + Vector2.right) * spawnVelocity;
            }
            else if (Level >= 4 && Random.value < thirdProjectileChance)
            {
                // Third Piece
                derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<SplitterSmallProjectile>();
                derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(Damage, Radius);
                derivedProjectile.SetExtraFields(Level == 6);
                derivedRb2d.velocity = Vector2.up * spawnVelocity;
            }
        }
    }
}
