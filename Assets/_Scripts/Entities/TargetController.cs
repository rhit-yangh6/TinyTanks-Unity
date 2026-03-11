using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Entities
{
    public class TargetController : EnemyController
    {
        // Skip EnemyController.Start() (which needs weapon/line renderer)
        // but still run BuffableEntity → Entity init for health, sprites, etc.
        protected override void Start()
        {
            Health = MaxHealth;
            healthBar.SetHealth(Health, MaxHealth);

            Rigidbody2D = GetComponent<Rigidbody2D>();
            ColliderSize = GetComponent<CapsuleCollider2D>().size;

            FacingDirection = 1;
            MainSpriteRenderer = GetComponent<SpriteRenderer>();
            if (TankCannon != null)
                CannonSpriteRenderer = TankCannon.GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            // Targets don't move or adjust rotation
        }

        public override IEnumerator MakeMove()
        {
            EventBus.Broadcast(EventTypes.EndTurn, (BuffableEntity)this);
            yield return null;
        }
    }
}
