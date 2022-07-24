using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/BurningBuff")]
    public class BurningBuff: ScriptableBuff
    {
        public float burningDamage;
        
        public override TimedBuff InitializeBuff(GameObject obj)
        {
            return new TimedBurningBuff(this, obj, duration);
        }
    }
    
    public class TimedBurningBuff : TimedBuff
    {
        private readonly BuffableEntity _be;

        public TimedBurningBuff(ScriptableBuff buff, GameObject obj, int duration) : base(buff, obj)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect() { }

        protected override void End() { }

        protected override void TurnTrigger()
        {
            BurningBuff buff = (BurningBuff)Buff;
            _be.TakeDamage(buff.burningDamage);
        }
    }
}