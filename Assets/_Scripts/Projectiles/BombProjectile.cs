using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class BombProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks secondaryExplosionFeedbacks;
        [SerializeField] private MMFeedbacks clusterExplosionFeedbacks;
        [SerializeField] private MMFeedbacks longerFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _clusterExplosionRadius, _clusterExplosionDamage;
        private static float _secondaryExplosionRadius, _secondaryExplosionDamage;

        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 4 ? _damage * 1.25f : _damage;
        protected override float MaxMagnitude => Level >= 3 ? _maxMagnitude * 1.1f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        // Other Variables
        private readonly int[,] _clusterDirections = 
        { { 2, 0 }, { 1, -1 }, { 0, -2 }, { -1, -1 }, {-2, 0}, {-1, 1}, {0, 2}, {1, 1} };
        
        private void Start()
        {
            switch (Level)
            {
                case 6:
                    clusterExplosionFeedbacks.PlayFeedbacks();
                    break;
                case 5:
                    if (Random.value < 0.35) secondaryExplosionFeedbacks.PlayFeedbacks();
                    else longerFeedbacks.PlayFeedbacks();
                    break;
                case >= 2:
                    longerFeedbacks.PlayFeedbacks();
                    break;
                default:
                    defaultMmFeedbacks.PlayFeedbacks();
                    break;
            }
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone")) Destroy(gameObject);
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            DealDamage();
        }

        public void SpawnSecondaryExplosion()
        {
            var origin = transform.position;
            
            DamageHandler.i.HandleDamage(origin, _secondaryExplosionRadius, _secondaryExplosionDamage, 
                DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, origin,
                _secondaryExplosionRadius, 1, DestroyTypes.Circular);
        }

        private IEnumerator SpawnClusterExplosion(Vector3 origin)
        {
            for (var i = 0; i < 8; i++)
            {
                var direction = Vector3.Normalize(Vector2.right * _clusterDirections[i, 0] +
                                                  Vector2.up * _clusterDirections[i, 1]);
                var pos = origin + direction * (Radius + _clusterExplosionRadius) / 2;
                
                DamageHandler.i.HandleDamage(pos, _clusterExplosionRadius, _clusterExplosionDamage,
                    DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                    _clusterExplosionRadius, 1, DestroyTypes.Circular);

                var insExpl = Instantiate(ExplosionFX, pos, Quaternion.identity);
                insExpl.transform.localScale *= _clusterExplosionRadius;
                Destroy(insExpl, ExplosionDuration);  
                
                yield return new WaitForSeconds(.07f);
            }

            yield return 0;
        }

        public void TriggerClusterExplosion()
        {
            StartCoroutine(SpawnClusterExplosion(transform.position));
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude,
            int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.explosionFX;

            _clusterExplosionDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "clusterExplosionDamage").value;
            _clusterExplosionRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "clusterExplosionRadius").value;
            
            _secondaryExplosionDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "secondaryExplosionDamage").value;
            _secondaryExplosionRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "secondaryExplosionRadius").value;
        }
    }
}
