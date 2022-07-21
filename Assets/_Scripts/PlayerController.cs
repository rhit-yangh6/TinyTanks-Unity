using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using UnityEngine;

// Source: https://github.com/Bardent/Rigidbody2D-Slopes-Unity/blob/master/Assets/Scripts/PlayerController.cs
namespace _Scripts
{
    public class PlayerController : MonoBehaviour, Character
    {
        public float Health { get; set; }

        [HideInInspector] public int facingDirection = 1;
        
        [SerializeField] public float maxHealth = 100;
        [SerializeField] public bool moveable;
        [SerializeField] public float movementSpeed = 5;
        [SerializeField] public LayerMask layerMask;
        [SerializeField] public PlayerHealthBarBehavior healthBar;
        [SerializeField] public GameObject tankCannon;
        
        private float _xInput;
        private SpriteRenderer _mainSr, _cannonSr;
        private Rigidbody2D _rb2d;

        private readonly Dictionary<ScriptableBuff, TimedBuff> _buffs = new ();
        
        private void Start()
        {
            Health = maxHealth;
            
            healthBar.SetHealth(Health, maxHealth);

            _mainSr = GetComponent<SpriteRenderer>();
            _cannonSr = tankCannon.GetComponent<SpriteRenderer>();

            _rb2d = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            CheckMovement();
            AdjustRotation();
        }
    
        public void TakeDamage(float amount)
        {
            if (Health - amount < 0)
            {
                Health = 0;
            }
            else
            {
                Health -= amount;
            }
            healthBar.SetHealth(Health, maxHealth);
        }

        public void AdjustRotation()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 3f, layerMask);

            if (hit.collider)
            {
                /*
            Debug.Log(Vector2.SignedAngle(hit.normal, Vector2.up));
            // Draw lines to show the incoming "beam" and the reflection.
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
            */
                float angle = Vector2.SignedAngle(hit.normal, Vector2.up);
                transform.eulerAngles = new Vector3 (0, 0, -angle);
            }
            else
            {
                transform.eulerAngles = new Vector3 (0, 0, 0);
            }
        
        }

        public void CheckMovement()
        {
            if (moveable)
            {
                _xInput = Input.GetAxisRaw("Horizontal");

                if (_xInput == 1 && facingDirection == -1)
                {
                    Flip();
                }
                else if (_xInput == -1 && facingDirection == 1)
                {
                    Flip();
                } else if (_xInput == 0)
                {
                    _rb2d.velocity = new Vector2(0, _rb2d.velocity.y);
                }

                transform.Translate(Time.deltaTime * movementSpeed * new Vector3(_xInput, 0, 0));
                
            }
        }
    
        public void Flip()
        {
            facingDirection *= -1;
            _mainSr.flipX = facingDirection == -1;
            _cannonSr.flipX = facingDirection == -1;
        }

        public void SetCannonAngle(float angle)
        {
            // TODO: Rotated Tank
            tankCannon.transform.localEulerAngles = (facingDirection == 1 ? -angle : (180 - angle)) * Vector3.forward;
        }

        public void AddBuff(TimedBuff buff)
        {
            if (_buffs.ContainsKey(buff.Buff))
            {
                _buffs[buff.Buff].Activate();
            }
            else
            {
                _buffs.Add(buff.Buff, buff);
                buff.Activate();
            }
        }

        public void TickBuffs()
        {
            foreach (var buff in _buffs.Values.ToList())
            {
                buff.Tick();
                if (buff.isFinished)
                {
                    _buffs.Remove(buff.Buff);
                }
            }
        }

        public void IncreaseMovementSpeed(float amount)
        {
            Debug.Log("Activated" + amount);
            movementSpeed += amount;
        }
    }
}
