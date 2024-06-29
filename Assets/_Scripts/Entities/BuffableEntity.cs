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
        
        protected readonly Dictionary<ScriptableBuff, TimedBuff> Buffs = new ();
        
        protected virtual GameObject TankCannon => tankCannon;
        protected virtual SpriteRenderer MainSr => null;
        protected virtual SpriteRenderer CannonSr => null;
        public virtual float MovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed = value;
        }

        protected virtual BuffPanelBehavior Bpb => bpb;

        protected override void CheckMovement() { }

        public void Flip()
        {
            FacingDirection *= -1;
            MainSr.flipX = FacingDirection == -1;
            CannonSr.flipX = FacingDirection == -1;
        }

        public void SetCannonAngle(float angle)
        {
            TankCannon.transform.eulerAngles = (FacingDirection == 1 ? -angle : (180 - angle)) * Vector3.forward;
        }

        public void AddBuff(TimedBuff buff)
        {
            if (Buffs.ContainsKey(buff.Buff))
            {
                Buffs[buff.Buff].Activate();
            }
            else
            {
                Buffs.Add(buff.Buff, buff);
                buff.Activate();
            }
            bpb.RefreshBuffDisplay(Buffs);
        }

        public void TickBuffs()
        {
            foreach (var buff in Buffs.Values.ToList())
            {
                buff.Tick();
                if (buff.isFinished)
                {
                    Buffs.Remove(buff.Buff);
                }
            }
            bpb.RefreshBuffDisplay(Buffs);
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone")) InstantDeath();
        }

        public void SelfExplode()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, 3, 50, DamageHandler.DamageType.Circular,true);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                3, 1, DestroyTypes.Circular);
            
            GameObject insExpl = Instantiate(GameAssets.i.regularExplosionFX, pos, Quaternion.identity);
            insExpl.transform.localScale *= 3;
            Destroy(insExpl, 0.5f);
        }

        public virtual void InstantDeath()
        {
            TakeDamage(MaxHealth * 2);
        }

        public ScriptableBuff GetBuff(string buffTypeName)
        {
            return Buffs.Keys.ToList().Find(k => k.GetType().Name.Equals(buffTypeName));
        }

        protected override void OnDeath()
        {
            var deathFX = Instantiate(GameAssets.i.deathFX, transform.position, Quaternion.identity);
            Destroy(deathFX, 3);
            EventBus.Broadcast(EventTypes.EndTurn, this);
        }
    }
}