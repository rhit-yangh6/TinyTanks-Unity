﻿using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/StunnedBuff")]
    public class StunnedBuff: ScriptableBuff
    {
        public override TimedBuff InitializeBuff(GameObject obj)
        {
            return new TimedStunnedBuff(this, obj, duration);
        }
    }
    
    public class TimedStunnedBuff : TimedBuff
    {
        private readonly BuffableEntity _be;

        public TimedStunnedBuff(ScriptableBuff buff, GameObject obj, int duration) : base(buff, obj)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect() { }

        protected override void End() { }

        protected override void TurnTrigger()
        {
            _be.EndTurn();
        }
    }
}