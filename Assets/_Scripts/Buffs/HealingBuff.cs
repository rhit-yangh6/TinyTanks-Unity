using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/HealingBuff")]
    public class HealingBuff: ScriptableBuff
    {
        public float healAmount;
        
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedHealingBuff(this, obj, level == 4 ? 999 : duration, level);
        }
    }
    
    public class TimedHealingBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private readonly float _finalHealAmount;

        public TimedHealingBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
            var healingBuff = (HealingBuff)Buff;
            _finalHealAmount = healingBuff.healAmount;
        }

        protected override void ApplyEffect() { }

        protected override void End() { }

        protected override void TurnTrigger()
        {
            _be.TakeDamage(-_finalHealAmount);
        }
    }
}