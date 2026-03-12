using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/SuperchargedBuff")]
    public class SuperchargedBuff: ScriptableBuff
    {
        public float coefficientMultiplier = 0.5f;
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedSuperchargedBuff(this, obj, duration, level);
        }
    }

    public class TimedSuperchargedBuff : TimedBuff
    {
        private readonly BuffableEntity _entity;

        public TimedSuperchargedBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            _entity = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect()
        {
            var superchargedBuff = (SuperchargedBuff) Buff;
            if (_entity != null)
            {
                _entity.FuelConsumptionCoefficient *= superchargedBuff.coefficientMultiplier;
            }
        }

        protected override void End()
        {
            var superchargedBuff = (SuperchargedBuff) Buff;
            if (_entity != null)
            {
                _entity.FuelConsumptionCoefficient /= superchargedBuff.coefficientMultiplier;
            }
        }

        protected override void TurnTrigger()
        {
            // Do nothing
        }
    }
}
