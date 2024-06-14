using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/OilyBuff")]
    public class OilyBuff: ScriptableBuff
    {
        public float burnDamageMultiplier = 1.5f; 
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedOilyBuff(this, obj, duration, level);
        }
    }
    
    public class TimedOilyBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private readonly float _finalBurningDamage;

        public TimedOilyBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            _be = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect() { }

        protected override void End() { }

        protected override void TurnTrigger()
        {
            // Do nothing
        }
    }
}