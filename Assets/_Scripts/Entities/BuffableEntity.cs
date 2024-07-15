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
        [SerializeField] public GameObject tankCannon;
        [SerializeField] protected BuffPanelBehavior bpb;

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
        
        protected override void Start()
        {
            base.Start();

            MainSpriteRenderer = GetComponent<SpriteRenderer>();
            MainSpriteRenderer.flipX = FacingDirection == -1;

            CannonSpriteRenderer = tankCannon.GetComponent<SpriteRenderer>();
            CannonSpriteRenderer.flipX = FacingDirection == -1;
        }

        protected override void CheckMovement() { }

        public void Flip()
        {
            FacingDirection *= -1;
            MainSpriteRenderer.flipX = FacingDirection == -1;
            CannonSpriteRenderer.flipX = FacingDirection == -1;
        }

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