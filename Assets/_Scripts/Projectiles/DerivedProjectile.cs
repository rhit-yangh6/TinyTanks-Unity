using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class DerivedProjectile : MonoBehaviour, IProjectile
    {
        public GameObject Shooter { get; set; }
        
        [SerializeField] protected MMFeedbacks defaultMmFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected virtual float Radius => _radius;
        protected virtual float Damage => _damage;
        protected virtual float ExplosionDuration => _explosionDuration;
        protected virtual GameObject ExplosionFX => _explosionFX;

        protected Rigidbody2D Rigidbody2D;
        protected Collider2D Collider2D;
        protected Renderer Renderer;
        protected bool IsDetonated;

        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            Collider2D = GetComponent<Collider2D>();
            Renderer = GetComponent<Renderer>();
        }
        
        protected void Spin()
        {
            if (IsDetonated) return;
            var velocity = Rigidbody2D.velocity;
            transform.Rotate(0,0, velocity.x > 0 ? -1 : 1);
        }

        protected void Direct()
        {
            var velocity = Rigidbody2D.velocity;
            var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public virtual void Detonate()
        {
            if (IsDetonated) return;
            IsDetonated = true;

            Disappear();
            DealDamage();
        
            defaultMmFeedbacks.PlayFeedbacks();
        }

        public virtual void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Circular);
        }
        
        protected void Disappear()
        {
            // Stop rigidBody from moving/rotating
            Rigidbody2D.gravityScale = 0;
            Rigidbody2D.freezeRotation = true;
            Rigidbody2D.velocity = Vector2.zero;

            // Disable collider
            Collider2D.enabled = false;
            
            // Stop rendering
            Renderer.enabled = false;
        }

        public virtual void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(ExplosionFX, transform.position, Quaternion.identity);
            insExpl.transform.localScale *= Radius;
            Destroy(insExpl, ExplosionDuration);
        }

        public virtual void DoCameraShake()
        {
            Camera.main.GetComponent<CameraMovement>().ShakeCamera(ExplosionDuration);
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


        public virtual void SetParameters(float damage, float radius, float explosionDuration, GameObject explosionFX) { }
    }
}