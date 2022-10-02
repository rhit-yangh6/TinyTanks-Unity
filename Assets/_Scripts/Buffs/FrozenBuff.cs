using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/FrozenBuff")]
    public class FrozenBuff: ScriptableBuff
    {
        // public float burningDamage;
        
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedFrozenBuff(this, obj, duration, level);
        }
    }
    
    public class TimedFrozenBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private float oldMovementSpeed;

        public TimedFrozenBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect()
        {
            oldMovementSpeed = _be.MovementSpeed;
            _be.MovementSpeed = 2f;
        }

        protected override void End()
        {
            _be.MovementSpeed = oldMovementSpeed;
        }

        protected override void TurnTrigger()
        { /* Do Nothing */ }
    }
}