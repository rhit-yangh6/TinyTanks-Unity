using System;
using System.Collections;
using _Scripts.Entities;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class JackhammerProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private float explosionTimeInterval = 0.23f;
        [SerializeField] private float bifurcateAngle = 15f;
        [SerializeField] private float earthquakeChance = 0.3f;
        [SerializeField] private MMFeedbacks bifurcateMmFeedbacks;
        [SerializeField] private MMFeedbacks earthquakeMmFeedbacks;
        [SerializeField] private float explosionNumber = 3f;
        [SerializeField] private float explosionRadius = 1.5f;
        [SerializeField] private float explosionDamage = 10f;
        [SerializeField] private float explosionDistance = 1.8f;
        
        // References
        protected override float Damage =>  Level >= 3 ? damage * 1.3f : damage;
        protected override float MaxMagnitude => Level >= 2 ? maxMagnitude * 1.16f : maxMagnitude;
        private float ExplosionDamage
        {
            get
            {
                return Level switch
                {
                    >= 3 => explosionDamage * 1.3f,
                    _ => explosionDamage
                };
            }
        }
        private float ExplosionRadius
        {
            get
            {
                return Level switch
                {
                    >= 3 => explosionRadius * 1.2f,
                    _ => explosionRadius
                };
            }
        }
        private int ExplosionNumber
        {
            get
            {
                return Level switch
                {
                    >= 4 => (int)explosionNumber + 1,
                    _ => (int)explosionNumber
                };
            }
        }
        
        // Other Variables
        private bool _isActivated, _recordedVector;
        private Vector2 _collisionDirection;
        
        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else
            {
                if (_recordedVector) return;
                _collisionDirection = -col.relativeVelocity.normalized;
                _recordedVector = true;
                Detonate();
            }
        }
        
        private void Update()
        {
            Direct();
        }
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            
            DealDamage();
            // Trigger Earthquake
            if (Level == 5 && Random.value < earthquakeChance)
            {
                earthquakeMmFeedbacks.PlayFeedbacks();
            }
            else if (Level == 6)
            {
                bifurcateMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public void TriggerEarthquake()
        {
            var entities = FindObjectsOfType<Entity>();
            foreach (var e in entities)
            {
                if (e.IsGrounded())
                {
                    e.TakeDamage(ExplosionDamage);
                }
            }
        }

        public void StartConsecutiveExplosions()
        {
            StartCoroutine(SpawnConsecutiveExplosions());
        }

        public void StartBifurcatedExplosions()
        {
            StartCoroutine(SpawnBifurcatedExplosions());
        }
        
        private IEnumerator SpawnConsecutiveExplosions()
        {
            var newPos = (Vector2)transform.position;
            for (var i = 0; i < ExplosionNumber; i++)
            {
                yield return new WaitForSeconds(explosionTimeInterval);
                newPos += _collisionDirection * explosionDistance;
                DamageHandler.i.HandleDamage(newPos, ExplosionRadius, ExplosionDamage, DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, (Vector3)newPos,
                    ExplosionRadius, 1, DestroyTypes.Circular);
                var insExpl = Instantiate(GameAssets.i.explosionFX, newPos, Quaternion.identity);
                Destroy(insExpl, explosionTimeInterval);
            }
        }

        private IEnumerator SpawnBifurcatedExplosions()
        {
            var pos = (Vector2)transform.position;
            var newPos1 = pos;
            var newPos2 = pos;
            var collisionDirection1 = Geometry.Rotate(_collisionDirection, bifurcateAngle);
            var collisionDirection2 = Geometry.Rotate(_collisionDirection, -bifurcateAngle);
            for (var i = 0; i < ExplosionNumber; i++)
            {
                yield return new WaitForSeconds(explosionTimeInterval);
                newPos1 += collisionDirection1 * explosionDistance;
                DamageHandler.i.HandleDamage(newPos1, ExplosionRadius, ExplosionDamage, DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, (Vector3)newPos1,
                    ExplosionRadius, 1, DestroyTypes.Circular);
                var insExpl = Instantiate(GameAssets.i.explosionFX, newPos1, Quaternion.identity);
                Destroy(insExpl, explosionTimeInterval);
                
                newPos2 += collisionDirection2 * explosionDistance;
                DamageHandler.i.HandleDamage(newPos2, ExplosionRadius, ExplosionDamage, DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, (Vector3)newPos2,
                    ExplosionRadius, 1, DestroyTypes.Circular);
                insExpl = Instantiate(GameAssets.i.explosionFX, newPos2, Quaternion.identity);
                Destroy(insExpl, explosionTimeInterval);
            }
        }
    }
}