using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Projectiles;
using _Scripts.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Entities
{
    public class TargetController : BuffableEntity
    {
        protected override float MaxHealth => maxHealth;
        public override float MovementSpeed => movementSpeed;
        protected override HealthBarBehavior HealthBar => healthBar;

        private void Start()
        {
            Health = maxHealth;
            HealthBar.SetHealth(Health, MaxHealth);
        }
    }
}
