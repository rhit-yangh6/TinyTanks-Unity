﻿using _Scripts.Entities;
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

        public TimedSpeedBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect()
        {
            //Add speed increase to MovementComponent
            SpeedBuff speedBuff = (SpeedBuff) Buff;
            
            // TODO: No Method Here? Need to finish
        }

        protected override void End()
        {
            // Revert speed increase
            SpeedBuff speedBuff = (SpeedBuff) Buff;
            // _entity.IncreaseMovementSpeed(-speedBuff.speedIncrease * effectStacks);
            effectStacks = 0;
        }
    }
}