using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using UnityEngine;

// Source: https://github.com/Bardent/Rigidbody2D-Slopes-Unity/blob/master/Assets/Scripts/PlayerController.cs
namespace _Scripts.Entities
{
    public class PlayerController : BuffableEntity
    {
        public bool moveable;

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

            _mainSr = GetComponent<SpriteRenderer>();
            _cannonSr = TankCannon.GetComponent<SpriteRenderer>();

            _rb2d = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            CheckMovement();
            AdjustRotation();
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

            transform.Translate(Time.deltaTime * movementSpeed * new Vector3(_xInput, 0, 0));
        }
    }
}
