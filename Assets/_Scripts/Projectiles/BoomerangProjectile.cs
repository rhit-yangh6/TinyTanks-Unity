using System;
using _Scripts.Entities;
using UnityEngine;
using Object = System.Object;

namespace _Scripts.Projectiles
{
    public class BoomerangProjectile : LaunchedProjectile
    { 
        // Set in Inspector
        [SerializeField] private LayerMask layerMask;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _addForceMultiplier, _selfTrackSpeedMultiplier, _extraForceXMultiplier, _extraForceYMultiplier;
        
        // References
        protected override float Radius => _radius;

        protected override float Damage
        {
            get
            {
                if (Level == 6) return _damage * 1.48f; // LEVEl 6
                return Level >= 4 ? _damage * 1.35f : _damage; // LEVEL 4+
            }
        }
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => Level >= 3 ? (int)(_steps * 1.4f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated;
        private Vector2 _aimingLocation;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            var velocity = _rb.velocity;
            _rb.GetComponent<ConstantForce2D>().force = new Vector3(velocity.x * _extraForceXMultiplier,
                velocity.y * _extraForceYMultiplier, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_isActivated && Level >= 5)
            {
                _isActivated = true;

                Vector2 startPos = transform.position;
                if (Level == 5)
                {
                    Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 angleVelocity = (endPos - startPos).normalized;
                    
                    // TODO: Multiplier subject to change
                    _rb.AddForce(angleVelocity * _addForceMultiplier);
                }
                else
                {
                    Vector2 endPos = Shooter.transform.position;
                    Vector2 angleVelocity = (endPos - startPos).normalized;

                    _rb.GetComponent<ConstantForce2D>().force = new Vector2();
                    _rb.gravityScale = -1;
                    _rb.velocity = angleVelocity * (_rb.velocity.magnitude * _selfTrackSpeedMultiplier);
                }
            }
        }
        
        public override void Detonate()
        {
            Vector2 pos = transform.position;

            // TODO: Maybe move this to the Damage Handler if this is becoming more popular
            if (Level >= 2)
            {
                var hitColliders = Physics2D.OverlapCircleAll(pos, Radius, layerMask);
            
                foreach(var col in hitColliders)
                {
                    var rb = col.GetComponent<Rigidbody2D>();
                    if (rb == null) continue;

                    // Skip if it is the shooter
                    if (ReferenceEquals(Shooter, rb.gameObject)) continue;
                    var e = rb.gameObject.GetComponent<Entity>();
                    e.TakeDamage((float)Math.Round(Damage), false);
                }
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            }

            TerrainDestroyer.instance.DestroyTerrainCircular(pos, Radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }
        
        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
            
            _addForceMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "addForceMultiplier").value;
            _selfTrackSpeedMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "selfTrackSpeedMultiplier").value;
            _extraForceXMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "extraForceXMultiplier").value;
            _extraForceYMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "extraForceYMultiplier").value;
        }
    }
}
