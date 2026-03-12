using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Entities
{
    public class PlayerController : BuffableEntity
    {
        public bool moveable;

        private float _xInput;

        protected override void Start()
        {
            healthBar = GameObject.FindGameObjectWithTag("UI").GetComponent<UI.HealthBarBehavior>();
            base.Start();
        }

        private void FixedUpdate()
        {
            CheckMovement();
            AdjustRotation();
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                SteamManager.UnlockAchievement(Constants.AchievementOops);
                TakeDamage(MaxHealth);
            }
        }

        protected override void CheckMovement()
        {
            if (!moveable || !HasFuel)
            {
                FreezeOrFall();
                return;
            }

            _xInput = Input.GetAxisRaw("Horizontal");
            FlipToMatch(_xInput);

            if (_xInput == 0)
            {
                FreezeOrFall();
                return;
            }

            ConsumeFuel();
            MoveOnSurface(_xInput);
        }
    }
}
