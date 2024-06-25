using System;
using _Scripts.Buffs;
using _Scripts.Managers;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Entities
{
    public abstract class Entity: MonoBehaviour
    {
        [SerializeField] [Range(0, 20f)] private float rotationTolerance;
        [SerializeField] protected LayerMask layerMask;

        public float Health { get; set; }
        public bool IsDead { get; set; }

        public float DamageMultiplier { get; set; } = 1.0f;

        [SerializeField] protected float maxHealth;
        [SerializeField] protected HealthBarBehavior healthBar;
        [SerializeField] protected Transform topLeft, bottomRight;

        protected virtual float MaxHealth => maxHealth;
        protected virtual HealthBarBehavior HealthBar => healthBar;

        protected float angle;
        protected Vector2 colliderSize;

        private void Awake()
        {
            colliderSize = GetComponent<CapsuleCollider2D>().size;
        }

        public virtual void TakeDamage(float amount, bool isCriticalHit = false)
        {
            if (amount == 0) return;

            amount *= DamageMultiplier;
            var rb = gameObject.GetComponent<Rigidbody2D>();
            var roundedDamageAmount = (int)Math.Round(amount);
            DamagePopup.Create(rb.position, roundedDamageAmount, isCriticalHit);
            
            // Broadcast DamageDealt Event
            EventBus.Broadcast(EventTypes.DamageDealt, roundedDamageAmount);
            
            if (Health - roundedDamageAmount < 0)
            {
                Health = 0;
            }
            else if (Health - roundedDamageAmount > MaxHealth)
            {
                Health = maxHealth;
            }
            else
            {
                Health -= roundedDamageAmount;
            }
            HealthBar.SetHealth(Health, MaxHealth);

            if (Health <= 0)
            {
                IsDead = true;
                Destroy(gameObject);
            }
        }

        public void CompleteHeal()
        {
            var healAmount = (int) (Health - maxHealth);

            if (healAmount == 0) return;
            
            var rb = gameObject.GetComponent<Rigidbody2D>();
            DamagePopup.Create(rb.position, healAmount, false);

            Health = maxHealth;
            HealthBar.SetHealth(Health, MaxHealth);
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                TakeDamage(MaxHealth);
            }
        }

        protected void AdjustRotation()
        {
            Vector2 leftCheckPos = transform.position - new Vector3(colliderSize.x / 2, 0);
            Vector2 rightCheckPos = transform.position + new Vector3(colliderSize.x / 2, 0);
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, layerMask);
            RaycastHit2D hitRight = Physics2D.Raycast(rightCheckPos, Vector2.down, 2f, layerMask);
            RaycastHit2D hitLeft = Physics2D.Raycast(leftCheckPos, Vector2.down, 2f, layerMask);

            if (hit.collider)
            {
                var hitCount = 1;
                var totalAngle = Vector2.SignedAngle(hit.normal, Vector2.up);

                if (hitRight.collider)
                {
                    totalAngle += Vector2.SignedAngle(hitRight.normal, Vector2.up);
                    hitCount += 1;
                }

                if (hitLeft.collider)
                {
                    totalAngle += Vector2.SignedAngle(hitLeft.normal, Vector2.up);
                    hitCount += 1;
                }

                var finalAngle = totalAngle / hitCount;

                transform.eulerAngles = Math.Abs(finalAngle - angle) > rotationTolerance ?
                    new Vector3 (0, 0, -finalAngle) :
                    new Vector3 (0, 0, -angle);
                angle = finalAngle;
            }
            else
            {
                transform.eulerAngles = new Vector3 (0, 0, 0);
            }
        }

        protected abstract void CheckMovement();
        
        public bool IsGrounded()
        {
            return Physics2D.OverlapArea(topLeft.position, 
                bottomRight.position, layerMask);
        }
    }
}