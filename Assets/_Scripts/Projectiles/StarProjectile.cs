using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class StarProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _damageMultiplier, _radiusMultiplier, _drawStarSpeed;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private TrailRenderer _tr;
        private ParticleSystem _ps;
        private bool _isActivated, _isStarComplete;
        private const float RotateDegree = -144f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _tr = GetComponent<TrailRenderer>();
            _ps = gameObject.GetComponentInChildren<ParticleSystem>();
            _ps.Stop();
        }
        
        private void Update()
        {
            if (!_isActivated) transform.Rotate(0, 0, _rb.velocity.x > 0 ? -1 : 1);
            else
            {
                var velocity = _rb.velocity;
                var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                StartCoroutine(DrawStar());
            }
        }

        private IEnumerator DrawStar()
        {
            var initialVelocity = _rb.velocity;
            _rb.gravityScale = 0;
            _rb.velocity = Vector2.zero;
            _tr.emitting = true;

            _rb.velocity = (Vector2.left + Vector2.down) * _drawStarSpeed;
            
            yield return new WaitForSeconds(0.4f);

            for (var i = 0; i < 4; i++)
            {
                _rb.velocity = Rotate(_rb.velocity, RotateDegree);
                yield return new WaitForSeconds(0.4f);
            }
            
            _rb.velocity = initialVelocity;
            _rb.gravityScale = 1;
            _tr.emitting = false;
            _ps.Play();
            _isStarComplete = true;
            yield return 0;
        }

        public override void Detonate()
        {
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleDamage(
                pos, 
                _isStarComplete ? Radius * _radiusMultiplier : Radius,
                _isStarComplete ? Damage * _damageMultiplier : Damage, 
                DamageHandler.DamageType.Circular);

            TerrainDestroyer.instance.DestroyTerrainCircular(pos, 
                _isStarComplete ? Radius * _radiusMultiplier : Radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
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

            _damageMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "damageMultiplier").value;
            _radiusMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "radiusMultiplier").value;
            _drawStarSpeed = Array.Find(extraWeaponTerms, ewt => ewt.term == "drawStarSpeed").value;
        }
        
        // TODO: Util Class?
        private Vector2 Rotate(Vector2 v, float delta)
        {
            var deltaRad = delta * Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(deltaRad) - v.y * Mathf.Sin(deltaRad),
                v.x * Mathf.Sin(deltaRad) + v.y * Mathf.Cos(deltaRad)
            );
        }
    }
}