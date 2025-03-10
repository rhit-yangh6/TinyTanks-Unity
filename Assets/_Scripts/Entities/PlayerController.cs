using System;
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
        
        private float _xInput;
        
        protected override void Start()
        {
            healthBar = GameObject.FindGameObjectWithTag("UI").GetComponent<HealthBarBehavior>();
            fuel = maxFuel;
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
            if (!moveable)
            {
                if (IsGrounded())
                {
                    Rigidbody2D.velocity = Vector2.zero;
                    Rigidbody2D.isKinematic = true;
                }
                else
                {
                    Rigidbody2D.isKinematic = false;
                }
                return;
            }
            
            // Cannot move if player has no fuel
            if (fuel <= 0)
            {
                Rigidbody2D.velocity = Vector2.zero;
                Rigidbody2D.isKinematic = true;
                return;
            }

            _xInput = Input.GetAxisRaw("Horizontal");
            Rigidbody2D.isKinematic = false;

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
                Rigidbody2D.velocity = Vector2.zero;
                Rigidbody2D.isKinematic = true;
            }

            if (_xInput != 0)
            {
                fuel -= Time.deltaTime * fuelConsumptionCoefficient;
            }
            
            if (IsGrounded())
            {
                transform.Translate(Time.deltaTime * movementSpeed * new Vector3(_xInput, 0, 0));
            }
            else
            {
                transform.Translate(Time.deltaTime * movementSpeed * new Vector3(_xInput, -1, 0));
            }
        }

        public void Refuel(float amount)
        {
            fuel = Math.Min(maxFuel, fuel + amount);
        }
    }
}
