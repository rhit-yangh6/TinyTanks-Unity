using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using _Scripts.GameEngine;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Entities
{
    public class BuffableEntity : Entity
    {
        public int FacingDirection { get; set; } = 1;

        [SerializeField] protected float movementSpeed;
        [SerializeField] private GameObject tankCannon;
        public GameObject TankCannon => tankCannon;
        [SerializeField] protected BuffPanelBehavior bpb;
        [SerializeField] private bool startFacingLeft;

        [Header("Fuel")]
        [SerializeField] protected float fuel = 100f;
        [SerializeField] protected float maxFuel = 100f;
        [SerializeField] protected float fuelConsumptionCoefficient = 10f;

        [Header("Slope")]
        [SerializeField] protected float maxSlopeAngle = 60f;
        [SerializeField] protected float slopeCheckDistance = 0.5f;

        public float Fuel => fuel;
        public float MaxFuel => maxFuel;

        private readonly Dictionary<ScriptableBuff, TimedBuff> _buffs = new ();
        public virtual float MovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed = value;
        }

        // Tank + Cannon Sprite Renderer
        protected SpriteRenderer MainSpriteRenderer, CannonSpriteRenderer;

        // Animation indices
        protected static readonly int Shoot1 = Animator.StringToHash("Shoot");

        // Distance from transform origin to the bottom of the capsule collider
        private float _groundOffset;

        protected override void Start()
        {
            base.Start();
            fuel = maxFuel;

            // Compute from collider geometry: bottom of capsule = offset.y - size.y/2
            var capsule = GetComponent<CapsuleCollider2D>();
            _groundOffset = capsule.size.y * 0.5f - capsule.offset.y;

            FacingDirection = startFacingLeft ? -1 : 1;

            MainSpriteRenderer = GetComponent<SpriteRenderer>();
            MainSpriteRenderer.flipX = startFacingLeft;

            CannonSpriteRenderer = tankCannon.GetComponent<SpriteRenderer>();
            CannonSpriteRenderer.flipX = startFacingLeft;
        }

        protected override void CheckMovement() { }

        #region Shared Movement

        public void Flip()
        {
            FacingDirection *= -1;
            MainSpriteRenderer.flipX = FacingDirection == -1;
            CannonSpriteRenderer.flipX = FacingDirection == -1;
        }

        protected void FlipToMatch(float horizontal)
        {
            if ((horizontal > 0 && FacingDirection == -1) ||
                (horizontal < 0 && FacingDirection == 1))
                Flip();
        }

        protected void MoveOnSurface(float horizontal)
        {
            if (IsSlopeTooSteep(horizontal)) return;

            // Stay kinematic — no physics velocity, no bouncing, no sliding
            Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            Rigidbody2D.linearVelocity = Vector2.zero;

            // Move horizontally, then snap to ground
            float dx = horizontal * movementSpeed * Time.deltaTime;
            Vector3 pos = transform.position;
            pos.x += dx;

            // Cast from above to find ground at the new X position
            RaycastHit2D hit = Physics2D.Raycast(
                new Vector2(pos.x, pos.y + 1f),
                Vector2.down,
                4f,
                layerMask);

            if (hit.collider)
            {
                pos.y = hit.point.y + _groundOffset;

                // Align rotation to surface normal
                float angle = Vector2.SignedAngle(hit.normal, Vector2.up);
                transform.eulerAngles = new Vector3(0, 0, -angle);
            }

            transform.position = pos;
        }

        protected bool IsSlopeTooSteep(float horizontal)
        {
            Vector2 checkPos = (Vector2)transform.position - new Vector2(0, ColliderSize.y / 3);
            Vector2 dir = horizontal > 0 ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(checkPos, dir, slopeCheckDistance, layerMask);
            return hit.collider && Vector2.Angle(hit.normal, Vector2.up) > maxSlopeAngle;
        }

        protected void FreezeOrFall()
        {
            if (IsGrounded())
            {
                Rigidbody2D.linearVelocity = Vector2.zero;
                Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                // Dynamic only for falling — physics gravity handles it,
                // immediately switches back to Kinematic when grounded
                Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        protected void ConsumeFuel()
        {
            fuel -= Time.deltaTime * fuelConsumptionCoefficient;
            fuel = Mathf.Max(fuel, 0);
        }

        public float FuelConsumptionCoefficient
        {
            get => fuelConsumptionCoefficient;
            set => fuelConsumptionCoefficient = value;
        }

        public void ResetFuel() => fuel = maxFuel;

        public void Refuel(float amount) => fuel = Math.Min(maxFuel, fuel + amount);

        public bool HasFuel => fuel > 0;

        #endregion

        public void SetCannonAngle(float newAngle)
        {
            tankCannon.transform.eulerAngles = (FacingDirection == 1 ? -newAngle : (180 - newAngle)) * Vector3.forward;
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
            bpb.RefreshBuffDisplay(_buffs);
        }

        private static readonly List<ScriptableBuff> _expiredBuffKeys = new();

        public void TickBuffs()
        {
            _expiredBuffKeys.Clear();
            foreach (var kvp in _buffs)
            {
                kvp.Value.Tick();
                if (kvp.Value.isFinished)
                {
                    _expiredBuffKeys.Add(kvp.Key);
                }
            }
            foreach (var key in _expiredBuffKeys)
            {
                _buffs.Remove(key);
            }
            bpb.RefreshBuffDisplay(_buffs);
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone")) InstantDeath();
        }

        public override void SelfExplode()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, 3, 50, DamageHandler.DamageType.Circular,true);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                3, 1, DestroyTypes.Circular);
            
            var insExpl = Instantiate(GameAssets.i.explosionFX, pos, Quaternion.identity);
            insExpl.transform.localScale *= 3;
            Destroy(insExpl, 0.5f);
        }

        public ScriptableBuff GetBuff(string buffTypeName)
        {
            return _buffs.Keys.ToList().Find(k => k.GetType().Name.Equals(buffTypeName));
        }

        protected override void OnDeath()
        {
            var deathFX = Instantiate(GameAssets.i.deathFX, transform.position, Quaternion.identity);
            Destroy(deathFX, 3);
            EventBus.Broadcast(EventTypes.EndTurn, this);
        }
    }
}