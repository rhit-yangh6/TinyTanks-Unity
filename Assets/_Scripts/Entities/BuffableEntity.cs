using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using UnityEngine;

namespace _Scripts.Entities
{
    public class BuffableEntity : Entity
    {
        public int FacingDirection { get; set; } = 1;

        private float _movementSpeed;
        private GameObject _tankCannon;
        [SerializeField] protected GameController gc;
        
        protected readonly Dictionary<ScriptableBuff, TimedBuff> Buffs = new ();

        protected virtual GameObject TankCannon => _tankCannon;
        protected virtual SpriteRenderer MainSr => null;
        protected virtual SpriteRenderer CannonSr => null;
        public virtual float MovementSpeed => _movementSpeed;

        protected override void CheckMovement() { }

        public void Flip()
        {
            FacingDirection *= -1;
            MainSr.flipX = FacingDirection == -1;
            CannonSr.flipX = FacingDirection == -1;
        }

        public void SetCannonAngle(float angle)
        {
            TankCannon.transform.localEulerAngles = (FacingDirection == 1 ? -angle : (180 - angle)) * Vector3.forward;
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
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                TakeDamage(MaxHealth);
                EndTurn();
            }
        }

        public void EndTurn()
        {
            gc.ChangeTurn();
        }

    }
}