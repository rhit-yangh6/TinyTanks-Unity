using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using _Scripts.Managers;
using _Scripts.UI;
using _Scripts.Utils;
using UnityEngine;

// Source: https://github.com/Bardent/Rigidbody2D-Slopes-Unity/blob/master/Assets/Scripts/PlayerController.cs
namespace _Scripts.Entities
{
    public class PlayerController : BuffableEntity
    {
        public bool moveable;
        public float fuel;
        [SerializeField] public float maxFuel = 100f;
        [SerializeField] public float fuelConsumptionCoefficient = 10f;

        protected override float MaxHealth => maxHealth;
        public override float MovementSpeed => movementSpeed;
        protected override HealthBarBehavior HealthBar => GameObject.FindGameObjectWithTag("UI").GetComponent<HealthBarBehavior>();
        protected override GameObject TankCannon => tankCannon;
        protected override SpriteRenderer MainSr => _mainSr;
        protected override SpriteRenderer CannonSr => _cannonSr;

        private float _xInput;
        private SpriteRenderer _mainSr, _cannonSr;
        private Rigidbody2D _rb2d;

        private readonly Dictionary<ScriptableBuff, TimedBuff> _buffs = new ();
        
        private void Start()
        {
            Health = MaxHealth;
            HealthBar.SetHealth(Health, MaxHealth);

            fuel = maxFuel;

            _mainSr = GetComponent<SpriteRenderer>();
            _cannonSr = TankCannon.GetComponent<SpriteRenderer>();

            _rb2d = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
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
            if (!moveable)
            {
                if (IsGrounded())
                {
                    _rb2d.velocity = Vector2.zero;
                    _rb2d.isKinematic = true;
                }
                else
                {
                    _rb2d.isKinematic = false;
                }
                return;
            }

            _xInput = Input.GetAxisRaw("Horizontal");
            _rb2d.isKinematic = false;

            // Cannot move if player has no fuel
            if (fuel <= 0) return;

            if (_xInput == 1 && FacingDirection == -1)
            {
                Flip();
            }
            else if (_xInput == -1 && FacingDirection == 1)
            {
                Flip();
            }
            else if (_xInput == 0 && IsGrounded())
            {
                _rb2d.velocity = Vector2.zero;
                _rb2d.isKinematic = true;
            }

            if (_xInput != 0)
            {
                fuel -= Time.deltaTime * fuelConsumptionCoefficient;
            }

            transform.Translate(Time.deltaTime * movementSpeed * new Vector3(_xInput, 0, 0));
        }
    }
}
