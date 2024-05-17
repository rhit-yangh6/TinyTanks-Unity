using System;
using System.Collections;
using _Scripts.Entities;
using _Scripts.GameEngine;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class JackhammerProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private float explosionTimeInterval = 0.03f;
        [SerializeField] private float bifurcateAngle = 15f;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _explosionNumber, _explosionRadius, _explosionDamage, _explosionDistance,
            _earthquakeChance, _earthquakeShakeDuration;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage =>  Level >= 3 ? _damage * 1.3f : _damage;
        protected override float MaxMagnitude => Level >= 2 ? _maxMagnitude * 1.16f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated, _recordedVector, _triggeredEarthquake;
        private Rigidbody2D _rb;
        private Renderer _r;
        private Vector2 _collisionDirection;
    
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
            
            _explosionDamage = Level switch
            {
                >= 3 => _explosionDamage * 1.3f,
                _ => _explosionDamage
            };
            
            _explosionRadius = Level switch
            {
                >= 3 => _explosionRadius * 1.2f,
                _ => _explosionRadius
            };
            
            _explosionNumber = Level switch
            {
                >= 4 => _explosionNumber + 1,
                _ => _explosionNumber
            };
        }
        
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
        
        public override void Detonate()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Circular);
            
            // Trigger Earthquake
            if (Level == 5 && Random.value < _earthquakeChance)
            {
                var entities = FindObjectsOfType<Entity>();
                foreach (var e in entities)
                {
                    if (e.IsGrounded())
                    {
                        e.TakeDamage(_explosionDamage);
                    }
                }
                _triggeredEarthquake = true;
            }
            
            SpawnExplosionFX();
            DoCameraShake();

            _rb.gravityScale = 0;
            _r.enabled = false;

            StartCoroutine(Level == 6 ? SpawnBifurcatedExplosions(pos) : SpawnConsecutiveExplosions(pos));
        }
        
        public override void DoCameraShake()
        {
            Camera.main.GetComponent<CameraMovement>().ShakeCamera(_triggeredEarthquake ?
                _earthquakeShakeDuration : ExplosionDuration);
        }
        
        private IEnumerator SpawnConsecutiveExplosions(Vector2 origin)
        {
            var newPos = origin;
            for (var i = 0; i < _explosionNumber; i++)
            {
                yield return new WaitForSeconds(ExplosionDuration + explosionTimeInterval);
                newPos += _collisionDirection * _explosionDistance;
                DamageHandler.i.HandleDamage(newPos, _explosionRadius, _explosionDamage, DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, (Vector3)newPos,
                    _explosionRadius, 1, DestroyTypes.Circular);
                var insExpl = Instantiate(ExplosionFX, newPos, Quaternion.identity);
                insExpl.transform.localScale *= _explosionRadius;
                Destroy(insExpl, ExplosionDuration);
                DoCameraShake();
            }
            
            Destroy(gameObject);
        }

        private IEnumerator SpawnBifurcatedExplosions(Vector2 origin)
        {
            var newPos1 = origin;
            var newPos2 = origin;
            var collisionDirection1 = Geometry.Rotate(_collisionDirection, bifurcateAngle);
            var collisionDirection2 = Geometry.Rotate(_collisionDirection, -bifurcateAngle);
            for (var i = 0; i < _explosionNumber; i++)
            {
                yield return new WaitForSeconds(ExplosionDuration + explosionTimeInterval);
                newPos1 += collisionDirection1 * _explosionDistance;
                DamageHandler.i.HandleDamage(newPos1, _explosionRadius, _explosionDamage, DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, (Vector3)newPos1,
                    _explosionRadius, 1, DestroyTypes.Circular);
                var insExpl = Instantiate(ExplosionFX, newPos1, Quaternion.identity);
                insExpl.transform.localScale *= _explosionRadius;
                Destroy(insExpl, ExplosionDuration);
                
                newPos2 += collisionDirection2 * _explosionDistance;
                DamageHandler.i.HandleDamage(newPos2, _explosionRadius, _explosionDamage, DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, (Vector3)newPos2,
                    _explosionRadius, 1, DestroyTypes.Circular);
                insExpl = Instantiate(ExplosionFX, newPos2, Quaternion.identity);
                insExpl.transform.localScale *= _explosionRadius;
                Destroy(insExpl, ExplosionDuration);
                DoCameraShake();
            }
            
            Destroy(gameObject);
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.regularExplosionFX;
            
            _explosionNumber = Array.Find(extraWeaponTerms, ewt => ewt.term == "explosionNumber").value;
            _explosionRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "explosionRadius").value;
            _explosionDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "explosionDamage").value;
            _explosionDistance = Array.Find(extraWeaponTerms, ewt => ewt.term == "explosionDistance").value;
            _earthquakeChance = Array.Find(extraWeaponTerms, ewt => ewt.term == "earthquakeChance").value;
            _earthquakeShakeDuration = Array.Find(extraWeaponTerms, ewt => ewt.term == "earthquakeShakeDuration").value;
        }
    }
}