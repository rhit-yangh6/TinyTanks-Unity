using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/SpeedBuff")]
    public class SpeedBuff: ScriptableBuff
    {
        public float speedIncrease;

        public override TimedBuff InitializeBuff(GameObject obj)
        {
            return new TimedSpeedBuff(this, obj, duration);
        }
    }
    
    public class TimedSpeedBuff : TimedBuff
    {
        private readonly Character _character;

        public TimedSpeedBuff(ScriptableBuff buff, GameObject obj, int duration) : base(buff, obj)
        {
            //Getting MovementComponent, replace with your own implementation
            _character = obj.GetComponent<Character>();
        }

        protected override void ApplyEffect()
        {
            //Add speed increase to MovementComponent
            SpeedBuff speedBuff = (SpeedBuff) Buff;
            
            // TODO: No Method Here?
            _character.IncreaseMovementSpeed(speedBuff.speedIncrease);
        }

        public override void End()
        {
            // Revert speed increase
            SpeedBuff speedBuff = (SpeedBuff) Buff;
            _character.IncreaseMovementSpeed(-speedBuff.speedIncrease * effectStacks);
            effectStacks = 0;
        }
    }
}