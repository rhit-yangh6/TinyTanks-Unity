using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using JetBrains.Annotations;
using TerraformingTerrain2d;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class BombProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _detonateTime, _clusterExplosionRadius, _clusterExplosionDamage;
        private static float _secondaryExplosionRadius, _secondaryExplosionDamage;

        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 4 ? _damage * 1.25f : _damage;
        protected override float MaxMagnitude => Level >= 3 ? _maxMagnitude * 1.1f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        // Other Variables
        private SpriteRenderer _sr;
        private readonly int[,] _clusterDirections = 
        { { 2, 0 }, { 1, -1 }, { 0, -2 }, { -1, -1 }, {-2, 0}, {-1, 1}, {0, 2}, {1, 1} };
        
        // TODO: Level 5???
        
        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            StartCoroutine(TickBomb());
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone")) Destroy(gameObject);
        }

        private IEnumerator TickBomb()
        {
            var finalCalculatedDetonateTime = _detonateTime;
            if (Level >= 2) finalCalculatedDetonateTime += 1f;

            Invoke(nameof(Detonate), finalCalculatedDetonateTime);
            while (true)
            {
                _sr.color = Color.red;
                yield return new WaitForSeconds(.1f);
                _sr.color = Color.white;
                yield return new WaitForSeconds(.1f);
            }
        }

        public override void Detonate()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Circular);
        
            SpawnExplosionFX();
            DoCameraShake();

            switch (Level)
            {
                case 6:
                    StartCoroutine(SpawnClusterExplosion(pos));
                    break;
                case 5:
                    var random = Random.value;
                    if (random > 0.65) StartCoroutine(SpawnSecondaryExplosion(pos));
                    else Destroy(gameObject);
                    break;
                default:
                    Destroy(gameObject);
                    break;
            }
        }

        private IEnumerator SpawnSecondaryExplosion(Vector3 origin)
        {
            _sr.enabled = false;
            yield return new WaitForSeconds(ExplosionDuration + .25f);
            
            DamageHandler.i.HandleDamage(origin, _secondaryExplosionRadius, _secondaryExplosionDamage, 
                DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, origin,
                _secondaryExplosionRadius, 1, DestroyTypes.Circular);
            
            var insExpl = Instantiate(ExplosionFX, origin, Quaternion.identity);
            insExpl.transform.localScale *= _secondaryExplosionRadius;
            Destroy(insExpl, ExplosionDuration);  
            
            Destroy(gameObject);
        }

        private IEnumerator SpawnClusterExplosion(Vector3 origin)
        {
            // Hide the bomb
            _sr.enabled = false;

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

            Destroy(gameObject);
            yield return 0;
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

            _detonateTime = Array.Find(extraWeaponTerms, ewt => ewt.term == "detonateTime").value;
            _clusterExplosionDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "clusterExplosionDamage").value;
            _clusterExplosionRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "clusterExplosionRadius").value;
            
            _secondaryExplosionDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "secondaryExplosionDamage").value;
            _secondaryExplosionRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "secondaryExplosionRadius").value;
        }
    }
}
