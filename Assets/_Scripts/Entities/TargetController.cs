using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Projectiles;
using _Scripts.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Entities
{
    public class TargetController : EnemyController
    {
        protected override void Start()
        {
            Health = MaxHealth;
            healthBar.SetHealth(Health, MaxHealth);
            
            Rigidbody2D = GetComponent<Rigidbody2D>();
            ColliderSize = GetComponent<CapsuleCollider2D>().size;
        }
        
        private void FixedUpdate()
        {
            // Do nothing
        }

        public override IEnumerator MakeMove()
        {
            EventBus.Broadcast(EventTypes.EndTurn, GetComponent<BuffableEntity>());
            yield return 0;
        }
    }
}
