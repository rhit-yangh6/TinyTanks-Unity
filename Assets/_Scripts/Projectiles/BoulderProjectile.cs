using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using Unity.Mathematics;
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
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => Level >= 2 ? _maxMagnitude * 1.2f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        public override void Detonate()
        {
            Vector2 pos = transform.position;

            // Gaining from Higher Levels
            var finalCalculatedRadius = Radius;
            var finalCalculatedDamage = Damage;
            
            if (Level >= 3)
            {
                finalCalculatedRadius *= 1.15f;
                finalCalculatedDamage *= 1.20f;
            }

            if (Level == 5)
            {
                finalCalculatedRadius *= 1.40f;
                finalCalculatedDamage *= 1.35f;
            }

            DamageHandler.i.HandleDamage(pos, finalCalculatedRadius, finalCalculatedDamage, 
                DamageHandler.DamageType.Circular);

            TerrainDestroyer.instance.DestroyTerrainCircular(pos, finalCalculatedRadius);
        
            SpawnExplosionFX();
            DoCameraShake();

            // Split into Pieces

            var spawnPieces = false;
            if (Level >= 4) spawnPieces = Random.value > 0.8;

            if (Level == 6) spawnPieces = true;

            if (spawnPieces)
            {
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

            Destroy(gameObject);
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
