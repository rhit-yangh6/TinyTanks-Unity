using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public abstract class Projectile: MonoBehaviour, IProjectile
    {
        public GameObject Shooter { get; set; }
        
        protected Collider2D projectileCollider;
        protected Rigidbody2D rigidBody2D;
        protected Renderer projectileRenderer;
        protected SpriteRenderer SpriteRenderer;
        protected bool isDetonated;
        
        [SerializeField] protected MMFeedbacks defaultMmFeedbacks;

        [SerializeField] protected MMFeedbacks spawnMmFeedbacks;
        
        // Shared Fields
        protected float radius, damage, maxMagnitude;
        protected int steps;
        
        // References
        protected virtual float Radius => radius;
        protected virtual float Damage => damage;
        protected virtual float MaxMagnitude => maxMagnitude;
        protected virtual int Steps => steps;
        
        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            projectileCollider = GetComponent<Collider2D>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            projectileRenderer = GetComponent<Renderer>();
        }

        protected virtual void Start()
        {
            // Play spawn effects
            if (spawnMmFeedbacks != null)
            {
                spawnMmFeedbacks.PlayFeedbacks();
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D col)
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

        public IEnumerator TemporarilyDisableCollider()
        {
            projectileCollider.enabled = false;
            yield return new WaitForSeconds(0.1f);
            projectileCollider.enabled = true;
        }
        
        public virtual void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();

            DealDamage();
            
            defaultMmFeedbacks.PlayFeedbacks();
        }

        public virtual void Activate() { /* Do nothing as default */ }

        public virtual void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Circular);
        }
        
        public virtual Tuple<Vector2, float> Disappear()
        {
            var velocity = rigidBody2D.linearVelocity;
            var gravity = rigidBody2D.gravityScale;
            // Stop rigidBody from moving/rotating
            rigidBody2D.gravityScale = 0;
            rigidBody2D.freezeRotation = true;
            rigidBody2D.linearVelocity = Vector2.zero;

            // Disable collider
            projectileCollider.enabled = false;
            
            // Stop rendering
            projectileRenderer.enabled = false;

            return Tuple.Create(velocity, gravity);
        }

        public virtual void Reappear(Tuple<Vector2, float> oldInfo)
        {
            // Resume rigidBody state
            rigidBody2D.gravityScale = oldInfo.Item2;
            rigidBody2D.linearVelocity = oldInfo.Item1;
            rigidBody2D.freezeRotation = false;
            
            // Re-enable collider
            projectileCollider.enabled = true;
            
            // Start Rendering again
            projectileRenderer.enabled = true;
        }

        public void Spin(float spinSpeed = 1)
        {
            if (isDetonated) return;
            var velocity = rigidBody2D.linearVelocity;
            transform.Rotate(0,0, velocity.x > 0 ? -spinSpeed : spinSpeed);
        }
        
        public void Direct()
        {
            var velocity = rigidBody2D.linearVelocity;
            var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        public void SetParameters(float setDamage, float setRadius, float setMaxMagnitude, int setSteps)
        {
            damage = setDamage;
            radius = setRadius;
            maxMagnitude = setMaxMagnitude;
            steps = setSteps;
        }
    }
}