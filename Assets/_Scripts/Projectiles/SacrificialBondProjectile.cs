using System;
using System.Collections;
using _Scripts.Entities;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SacrificialBondProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private float enemyDamage = 20f;
        [SerializeField] private float healingDamage = 15f;
        [SerializeField] private float totalDistributedDamage = 55f;
        [SerializeField] private MMFeedbacks damageAndHealMmFeedbacks;
        
        // References
        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    6 => damage * 1.2f,
                    >= 2 => damage * 0.72f,
                    _ => damage
                };
            }
        }
        private float EnemyDamage
        {
            get
            {
                return Level switch
                {
                    6 => enemyDamage * 2f,
                    >= 3 => enemyDamage * 1.4f,
                    _ => enemyDamage
                };
            }
        }
        
        protected override void Start()
        {
            base.Start();
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.gravityScale = 0;
            renderer.enabled = false;

            if (Level >= 4)
            {                
                damageAndHealMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void DealDamage()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                if (Level == 5)
                {
                    enemy.GetComponent<BuffableEntity>().TakeDamage(totalDistributedDamage / enemies.Length);
                }
                else
                {
                    enemy.GetComponent<BuffableEntity>().TakeDamage(EnemyDamage);
                }
            }
            // Sacrifice self, it is always critical hit
            Shooter.GetComponent<BuffableEntity>().TakeDamage(Damage, true);
        }

        public void HealAfterDamage()
        { 
            Shooter.GetComponent<BuffableEntity>().TakeDamage(-healingDamage);
        }
    }
}