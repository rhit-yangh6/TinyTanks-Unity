using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class BoulderProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject boulderPiecePrefab;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _boulderPieceRadius, _boulderPieceDamage;
        
        // References
        protected override float Radius
        {
            get
            {
                return Level switch
                {
                    5 => _radius * 1.30f,
                    >= 3 => _radius * 1.15f,
                    _ => _radius
                };
            }
        }

        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    5 => _damage * 1.50f,
                    >= 3 => _damage * 1.20f,
                    _ => _damage
                };
            }
        }

        protected override float MaxMagnitude => Level >= 2 ? _maxMagnitude * 1.2f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

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
            }
        }

        private void SpawnPieces()
        {
            var pos = transform.position;
            // First Piece
            var derivedObject = Instantiate(boulderPiecePrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(_boulderPieceDamage, _boulderPieceRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = (Vector2.left + Vector2.up * 2) * 3f;
            
            // Second Piece
            derivedObject = Instantiate(boulderPiecePrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(_boulderPieceDamage, _boulderPieceRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = (Vector2.right + Vector2.up * 2) * 3f;
        }

        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;

            _boulderPieceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceDamage").value;
            _boulderPieceRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceRadius").value;
        }
    }
}
