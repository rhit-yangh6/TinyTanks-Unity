﻿using System;
using _Scripts.Buffs;
using UnityEngine;

namespace _Scripts.Entities
{
    public abstract class Entity: MonoBehaviour
    {
        
        [SerializeField] protected LayerMask layerMask;

        public float Health { get; set; }
        public bool IsDead { get; set; } = false;

        [SerializeField] protected float maxHealth;
        [SerializeField] protected HealthBarBehavior healthBar;

        protected virtual float MaxHealth => maxHealth;
        protected virtual HealthBarBehavior HealthBar => healthBar;

        public virtual void TakeDamage(float amount, bool isCriticalHit = false)
        {
            var rb = gameObject.GetComponent<Rigidbody2D>();
            DamagePopup.Create(rb.position, (int)Math.Round(amount), isCriticalHit);
            
            if (Health - amount < 0)
            {
                Health = 0;
            }
            else if (Health - amount > MaxHealth)
            {
                Health = maxHealth;
            }
            else
            {
                Health -= amount;
            }
            HealthBar.SetHealth(Health, MaxHealth);

            if (Health <= 0)
            {
                IsDead = true;
                Destroy(gameObject);
            }
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 3f, layerMask);

            if (hit.collider)
            {
                float angle = Vector2.SignedAngle(hit.normal, Vector2.up);
                transform.eulerAngles = new Vector3 (0, 0, -angle);
            }
            else
            {
                transform.eulerAngles = new Vector3 (0, 0, 0);
            }
        }

        protected virtual void CheckMovement() { }
    }
}