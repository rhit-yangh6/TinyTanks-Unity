using System;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/FrozenBuff")]
    public class FrozenBuff: ScriptableBuff
    {
        public float damageMultiplierFirstTier, damageMultiplierSecondTier, firstTierMovementSpeed;
        public float firstTierMovementSpeedMultiplier = 0.5f;
        public float secondTierMovementSpeedMultiplier = 0.01f;
        
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            // Level 4 = permafrost
            
            return new TimedFrozenBuff(this, obj, level switch
            {
                4 => 999,
                5 => duration + 2,
                _ => duration
            }, level);
        }
    }
    
    public class TimedFrozenBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private readonly float _finalDamageMultiplier, _finalMovementSpeedMultiplier;

        public TimedFrozenBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            _be = obj.GetComponent<BuffableEntity>();
            
            var frozenBuff = (FrozenBuff) Buff;
            _finalDamageMultiplier = level switch
            {
                5 => frozenBuff.damageMultiplierSecondTier,
                >= 2 => frozenBuff.damageMultiplierFirstTier,
                1 => 1.0f,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };

            _finalMovementSpeedMultiplier = level switch
            {
                >= 3 => frozenBuff.secondTierMovementSpeedMultiplier,
                _ => frozenBuff.firstTierMovementSpeedMultiplier
            };
        }

        protected override void ApplyEffect()
        {
            _be.MovementSpeed *= _finalMovementSpeedMultiplier;

            _be.DamageMultiplier *= _finalDamageMultiplier;
        }

        protected override void End()
        {
            _be.MovementSpeed /= _finalMovementSpeedMultiplier;
            
            _be.DamageMultiplier /= _finalDamageMultiplier;
        }

        protected override void TurnTrigger()
        { /* Do Nothing */ }
    }
}