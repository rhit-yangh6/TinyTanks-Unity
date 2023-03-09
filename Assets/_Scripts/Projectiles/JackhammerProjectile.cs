using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class JackhammerProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _explosionNumber, _explosionRadius, _explosionDamage, _explosionDistance;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated, _recordedVector;
        private Rigidbody2D _rb;
        private Renderer _r;
        private Vector2 _collisionDirection;
    
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
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
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);

            TerrainDestroyer.instance.DestroyTerrainCircular(pos, Radius);
        
            SpawnExplosionFX();
            DoCameraShake();

            _rb.gravityScale = 0;
            _r.enabled = false;
            
            StartCoroutine(SpawnConsecutiveExplosions(pos));
        }
        
        private IEnumerator SpawnConsecutiveExplosions(Vector2 origin)
        {
            var newPos = origin;
            for (var i = 0; i < _explosionNumber; i++)
            {
                yield return new WaitForSeconds(ExplosionDuration + .03f);
                newPos += _collisionDirection * _explosionDistance;
                DamageHandler.i.HandleDamage(newPos, _explosionRadius, _explosionDamage, DamageHandler.DamageType.Circular);
                TerrainDestroyer.instance.DestroyTerrainCircular(newPos, _explosionRadius);
                var insExpl = Instantiate(ExplosionFX, newPos, Quaternion.identity);
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
        }
    }
}