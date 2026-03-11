using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/SpeedBuff")]
    public class SpeedBuff: ScriptableBuff
    {
        public float speedIncrease;

        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedSpeedBuff(this, obj, duration, level);
        }
    }

    public class TimedSpeedBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private readonly float _speedIncrease;

        public TimedSpeedBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            _be = obj.GetComponent<BuffableEntity>();
            _speedIncrease = ((SpeedBuff)Buff).speedIncrease;
        }

        protected override void ApplyEffect()
        {
            _be.MovementSpeed += _speedIncrease * effectStacks;
        }

        protected override void End()
        {
            _be.MovementSpeed -= _speedIncrease * effectStacks;
            effectStacks = 0;
        }
    }
}
