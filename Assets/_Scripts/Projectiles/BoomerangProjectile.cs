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
        [SerializeField] private float addForceMultiplier = 555f;
        [SerializeField] private float selfTrackSpeedMultiplier = 2.55f;
        [SerializeField] private float extraForceXMultiplier = -0.5f;
        [SerializeField] private float extraForceYMultiplier = -0.25f;
        
        // References
        protected override float Damage
        {
            get
            {
                if (Level == 6) return damage * 1.48f; // LEVEl 6
                return Level >= 4 ? damage * 1.35f : damage; // LEVEL 4+
            }
        }
        protected override int Steps => Level >= 3 ? (int)(steps * 1.4f) : steps;

        // Other Variables
        private bool _isActivated;

        private void Start()
        {
            var velocity = rigidBody2D.velocity;
            rigidBody2D.GetComponent<ConstantForce2D>().force =
                new Vector3(velocity.x * extraForceXMultiplier, velocity.y * extraForceYMultiplier, 0);
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
                rigidBody2D.AddForce(angleVelocity * addForceMultiplier);
            }
            else
            {
                Vector2 endPos = Shooter.transform.position;
                Vector2 angleVelocity = (endPos - startPos).normalized;

                Destroy(rigidBody2D.GetComponent<ConstantForce2D>());
                rigidBody2D.gravityScale = 0;
                // TODO: have a minimum speed
                rigidBody2D.velocity = angleVelocity * (rigidBody2D.velocity.magnitude * selfTrackSpeedMultiplier);
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
    }
}
