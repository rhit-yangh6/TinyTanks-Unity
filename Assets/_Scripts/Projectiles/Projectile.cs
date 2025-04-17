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
        
        protected new Collider2D collider2D;
        protected Rigidbody2D rigidBody2D;
        protected new Renderer renderer;
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
            collider2D = GetComponent<Collider2D>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            renderer = GetComponent<Renderer>();
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
            collider2D.enabled = false;
            yield return new WaitForSeconds(0.1f);
            collider2D.enabled = true;
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
            var velocity = rigidBody2D.velocity;
            var gravity = rigidBody2D.gravityScale;
            // Stop rigidBody from moving/rotating
            rigidBody2D.gravityScale = 0;
            rigidBody2D.freezeRotation = true;
            rigidBody2D.velocity = Vector2.zero;

            // Disable collider
            collider2D.enabled = false;
            
            // Stop rendering
            renderer.enabled = false;

            return Tuple.Create(velocity, gravity);
        }

        public virtual void Reappear(Tuple<Vector2, float> oldInfo)
        {
            // Resume rigidBody state
            rigidBody2D.gravityScale = oldInfo.Item2;
            rigidBody2D.velocity = oldInfo.Item1;
            rigidBody2D.freezeRotation = false;
            
            // Re-enable collider
            collider2D.enabled = true;
            
            // Start Rendering again
            renderer.enabled = true;
        }

        public void Spin(float spinSpeed = 1)
        {
            if (isDetonated) return;
            var velocity = rigidBody2D.velocity;
            transform.Rotate(0,0, velocity.x > 0 ? -spinSpeed : spinSpeed);
        }
        
        public void Direct()
        {
            var velocity = rigidBody2D.velocity;
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