using System;
using System.Collections;
using _Scripts.Buffs;
using _Scripts.Managers;
using _Scripts.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Entities
{
    public abstract class Entity: MonoBehaviour
    {
        [SerializeField] protected LayerMask layerMask;

        public float Health { get; set; }
        public bool IsDead { get; set; }
        public float DamageMultiplier { get; set; } = 1.0f;

        [SerializeField] protected float maxHealth;
        [SerializeField] protected HealthBarBehavior healthBar;
        [SerializeField] protected Transform topLeft, bottomRight;

        protected virtual float MaxHealth => maxHealth;
        
        // Rb2D
        protected Rigidbody2D Rigidbody2D;
        
        // Capsule Collider Size
        protected Vector2 ColliderSize;

        protected virtual void Start()
        {
            Health = MaxHealth;
            healthBar.SetHealth(Health, MaxHealth);
            
            Rigidbody2D = GetComponent<Rigidbody2D>();
            ColliderSize = GetComponent<CapsuleCollider2D>().size;
        }

        public virtual void TakeDamage(float amount, bool isCriticalHit = false)
        {
            if (amount == 0) return;

            amount *= DamageMultiplier;
            var roundedDamageAmount = (int)Math.Round(amount);
            DamagePopup.Create(Rigidbody2D.position, roundedDamageAmount, isCriticalHit);

            // Broadcast DamageDealt Event
            EventBus.Broadcast(EventTypes.DamageDealt, roundedDamageAmount);

            if (Health - roundedDamageAmount < 0)
            {
                Health = 0;
            }
            else if (Health - roundedDamageAmount > MaxHealth)
            {
                Health = MaxHealth;
            }
            else
            {
                Health -= roundedDamageAmount;
            }
            healthBar.SetHealth(Health, MaxHealth);

            if (Health <= 0)
            {
                IsDead = true;
                OnDeath();
                Destroy(gameObject);
            }
        }

        public void CompleteHeal()
        {
            var healAmount = (int)(MaxHealth - Health);

            if (healAmount == 0) return;

            DamagePopup.Create(Rigidbody2D.position, -healAmount, false);

            Health = MaxHealth;
            healthBar.SetHealth(Health, MaxHealth);
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
            RaycastHit2D hit = Physics2D.Raycast(
                (Vector2)transform.position + Vector2.up * 0.5f,
                Vector2.down, 3f, layerMask);

            if (hit.collider)
            {
                float targetAngle = -Vector2.SignedAngle(hit.normal, Vector2.up);
                // Smooth rotation to avoid jitter
                float current = transform.eulerAngles.z;
                if (current > 180f) current -= 360f;
                float smoothed = Mathf.LerpAngle(current, targetAngle, 10f * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, smoothed);
            }
            else
            {
                float current = transform.eulerAngles.z;
                if (current > 180f) current -= 360f;
                float smoothed = Mathf.LerpAngle(current, 0f, 10f * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, smoothed);
            }
        }

        protected abstract void CheckMovement();
        
        public bool IsGrounded()
        {
            return Physics2D.OverlapArea(topLeft.position, 
                bottomRight.position, layerMask);
        }

        protected virtual void OnDeath()
        {
            // Do nothing as default
        }

        public virtual void SelfExplode()
        {
            // Do nothing as default
        }
        
        public virtual void InstantDeath()
        {
            TakeDamage(MaxHealth * 2);
        }

        public float GetHealthPercentage()
        {
            return Health / MaxHealth;
        }

        /// <summary>
        /// Display damage visuals (popup, health bar update) without re-running damage logic.
        /// Used by multiplayer clients who receive health updates via NetworkVariable.
        /// </summary>
        public virtual void TakeDamageVisualOnly(float amount, bool isCriticalHit)
        {
            var roundedDamageAmount = (int)System.Math.Round(amount);
            DamagePopup.Create(Rigidbody2D.position, roundedDamageAmount, isCriticalHit);
            healthBar.SetHealth(Health, MaxHealth);
        }
    }
}