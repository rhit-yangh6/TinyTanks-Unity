using System;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/FrozenBuff")]
    public class FrozenBuff: ScriptableBuff
    {
        public float damageMultiplierFirstTier, damageMultiplierSecondTier;
        
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            // Level 3 = permafrost
            return new TimedFrozenBuff(this, obj, level == 3 ? 999 : duration, level);
        }
    }
    
    public class TimedFrozenBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private float oldMovementSpeed;
        private readonly float finalDamageMultiplier;

        public TimedFrozenBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            _be = obj.GetComponent<BuffableEntity>();
            
            var frozenBuff = (FrozenBuff) Buff;
            finalDamageMultiplier = level switch
            {
                >= 3 => frozenBuff.damageMultiplierSecondTier,
                2 => frozenBuff.damageMultiplierFirstTier,
                1 => 1.0f,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }

        protected override void ApplyEffect()
        {
            oldMovementSpeed = _be.MovementSpeed;
            _be.MovementSpeed = 2f;

            _be.DamageMultiplier *= finalDamageMultiplier;
        }

        protected override void End()
        {
            _be.MovementSpeed = oldMovementSpeed;
            
            _be.DamageMultiplier /= finalDamageMultiplier;
        }

        protected override void TurnTrigger()
        { /* Do Nothing */ }
    }
}