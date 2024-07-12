using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class BoulderProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject boulderPiecePrefab;
        [SerializeField] private float boulderPieceRadius = 2.0f;
        [SerializeField] private float boulderPieceDamage = 15.0f;
        
        // References
        protected override float Radius
        {
            get
            {
                return Level switch
                {
                    5 => radius * 1.30f,
                    >= 3 => radius * 1.15f,
                    _ => radius
                };
            }
        }

        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    5 => damage * 1.50f,
                    >= 3 => damage * 1.20f,
                    _ => damage
                };
            }
        }

        protected override float MaxMagnitude => Level >= 2 ? maxMagnitude * 1.2f : maxMagnitude;
        
        private void Update()
        {
            Spin();
        }
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();
            
            DealDamage();
        
            defaultMmFeedbacks.PlayFeedbacks();

            // Split into Pieces
            var spawnPieces = false;
            if (Level >= 4) spawnPieces = Random.value > 0.8;
            if (Level == 6) spawnPieces = true;

            if (spawnPieces)
            {
                SpawnPieces();
                var splitCount = SteamManager.IncrementStat(Constants.StatBoulderSplitCount);
                if (splitCount >= 20)
                {
                    SteamManager.UnlockAchievement(Constants.AchievementPackThemUp);
                    WeaponManager.UnlockWeapon(34); // Packed Stone 34
                }
            }
        }

        private void SpawnPieces()
        {
            var pos = transform.position;
            // First Piece
            var derivedObject = Instantiate(boulderPiecePrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(boulderPieceDamage, boulderPieceRadius);
            derivedRb2d.velocity = (Vector2.left + Vector2.up * 2) * 3f;
            
            // Second Piece
            derivedObject = Instantiate(boulderPiecePrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(boulderPieceDamage, boulderPieceRadius);
            derivedRb2d.velocity = (Vector2.right + Vector2.up * 2) * 3f;
        }
    }
}
