using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class BoulderProjectile : MonoBehaviour, IProjectile
    {
        public int Level { get; set; }
        
        [SerializeField] private GameObject explosionFX;
        [SerializeField] private GameObject boulderPiecePrefab;
        
        private static float _radius, _damage, _maxMagnitude;
        private static int _steps;
        private static float _boulderPieceRadius, _boulderPieceDamage;
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else
            {
                Detonate();
            }
        }
        
        public void Detonate()
        {
            Vector2 pos = transform.position;

            // Gaining from Higher Levels
            var finalCalculatedRadius = _radius;
            var finalCalculatedDamage = _damage;
            
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

            DamageHandler.i.HandleCircularDamage(pos, finalCalculatedRadius, finalCalculatedDamage);

            TerrainDestroyer.Instance.DestroyTerrain(pos, finalCalculatedRadius);
        
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
                var derivedProjectile = derivedObject.GetComponent<IProjectile>();
                var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(_boulderPieceDamage, _boulderPieceRadius, 0, 0, null);
                derivedRb2d.velocity = (Vector2.left + Vector2.up * 2) * 3f;
            
                // Second Piece
                derivedObject = Instantiate(boulderPiecePrefab, pos, Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<IProjectile>();
                derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(_boulderPieceDamage, _boulderPieceRadius, 0, 0, null);
                derivedRb2d.velocity = (Vector2.right + Vector2.up * 2) * 3f;
            }

            Destroy(gameObject);
        }
    
        public void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(explosionFX, transform.position, quaternion.identity);
            insExpl.transform.localScale *= _radius;
            Destroy(insExpl, .2f);
        }

        public void DoCameraShake()
        {
            Camera.main.GetComponent<CameraShake>().shakeDuration = 0.2f;
        }

        public void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            
            _boulderPieceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceDamage").value;
            _boulderPieceRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceRadius").value;
        }

        public float GetMaxMagnitude()
        {
            var finalCalculatedMagnitude = _maxMagnitude;
            
            // TODO: Maybe there's a better practice?
            if (PlayerData.Instance.GetWeaponLevelFromId(1) >= 2)
            {
                finalCalculatedMagnitude *= 1.20f;
            }
            
            return finalCalculatedMagnitude;
        }

        public int GetSteps()
        {
            return _steps;
        }

        public float GetFixedMagnitude()
        {
            return -1f;
        }
    }

    
}
