using _Scripts.Entities;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/StunnedBuff")]
    public class StunnedBuff: ScriptableBuff
    {
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedStunnedBuff(this, obj, duration, level);
        }
    }
    
    public class TimedStunnedBuff : TimedBuff
    {
        private readonly BuffableEntity _be;

        public TimedStunnedBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect() { }

        protected override void End() { }

        protected override void TurnTrigger()
        {
            EventBus.Broadcast(EventTypes.EndTurn, _be);
        }
    }
}