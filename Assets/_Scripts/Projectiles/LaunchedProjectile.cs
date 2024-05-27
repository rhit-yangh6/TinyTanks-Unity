using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class LaunchedProjectile : MonoBehaviour, IProjectile
    {
        public int Level { get; set; }
        public GameObject Shooter { get; set; }

        [SerializeField] protected MMFeedbacks defaultMmFeedbacks;

        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected virtual float Radius => _radius;
        protected virtual float Damage => _damage;
        protected virtual float MaxMagnitude => _maxMagnitude;
        protected virtual int Steps => _steps;
        protected virtual float ExplosionDuration => _explosionDuration;
        protected virtual GameObject ExplosionFX => _explosionFX;
        
        // Other Fields
        protected Collider2D Collider2D;
        protected Rigidbody2D Rigidbody2D;
        protected Renderer renderer;
        protected bool isDetonated;

        private void Awake()
        {
            Collider2D = GetComponent<Collider2D>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
            renderer = GetComponent<Renderer>();
            StartCoroutine(TemporarilyDisableCollider());
        }

        private IEnumerator TemporarilyDisableCollider()
        {
            Collider2D.enabled = false;
            yield return new WaitForSeconds(0.1f);
            Collider2D.enabled = true;
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

        // The most basic detonate function
        public virtual void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

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

        protected virtual void Disappear()
        {
            // Stop rigidBody from moving/rotating
            Rigidbody2D.gravityScale = 0;
            Rigidbody2D.freezeRotation = true;
            Rigidbody2D.velocity = Vector2.zero;

            // Disable collider
            Collider2D.enabled = false;
            
            // Stop rendering
            renderer.enabled = false;
        }

        protected void Spin()
        {
            if (isDetonated) return;
            var velocity = Rigidbody2D.velocity;
            transform.Rotate(0,0, velocity.x > 0 ? -1 : 1);
        }
        
        protected void Direct()
        {
            var velocity = Rigidbody2D.velocity;
            var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public virtual void Activate() { /* Do nothing as default */ }
        
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

        public virtual void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms) { }

        public virtual float GetMaxMagnitude()
        {
            return MaxMagnitude;
        }

        public virtual int GetSteps()
        {
            return Steps;
        }

        public virtual float GetFixedMagnitude()
        {
            return -1f;
        }
    }
}
