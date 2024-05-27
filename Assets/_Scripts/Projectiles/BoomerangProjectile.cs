using System;
using _Scripts.Entities;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoomerangProjectile : LaunchedProjectile
    { 
        // Set in Inspector
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        
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

        private void Start()
        {
            var velocity = Rigidbody2D.velocity;
            Rigidbody2D.GetComponent<ConstantForce2D>().force =
                new Vector3(velocity.x * _extraForceXMultiplier, velocity.y * _extraForceYMultiplier, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_isActivated && Level >= 5)
            {
                _isActivated = true;
                activateMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            Vector2 startPos = transform.position;
            if (Level == 5)
            {
                Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 angleVelocity = (endPos - startPos).normalized;
                    
                // TODO: Multiplier subject to change
                Rigidbody2D.AddForce(angleVelocity * _addForceMultiplier);
            }
            else
            {
                Vector2 endPos = Shooter.transform.position;
                Vector2 angleVelocity = (endPos - startPos).normalized;

                Destroy(Rigidbody2D.GetComponent<ConstantForce2D>());
                Rigidbody2D.gravityScale = 0;
                // TODO: have a minimum speed
                Rigidbody2D.velocity = angleVelocity * (Rigidbody2D.velocity.magnitude * _selfTrackSpeedMultiplier);
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
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
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Circular);
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
