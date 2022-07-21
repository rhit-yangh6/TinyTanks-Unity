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
        private readonly Character _character;

        public TimedStunnedBuff(ScriptableBuff buff, GameObject obj, int duration) : base(buff, obj)
        {
            //Getting MovementComponent, replace with your own implementation
            _character = obj.GetComponent<Character>();
        }

        protected override void ApplyEffect()
        {
            StunnedBuff stunnedBuff = (StunnedBuff) Buff;
            // _movementComponent.MovementSpeed += speedBuff.SpeedIncrease;
        }

        public override void End()
        {
            // ScriptableSpeedBuff speedBuff = (ScriptableSpeedBuff) Buff;
            // _movementComponent.MovementSpeed -= speedBuff.SpeedIncrease * EffectStacks;
            // EffectStacks = 0;
        }
    }
}