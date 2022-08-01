using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/CursedBuff")]
    public class CursedBuff: ScriptableBuff
    {
        public override TimedBuff InitializeBuff(GameObject obj)
        {
            return new TimedCursedBuff(this, obj, duration);
        }
    }
    
    public class TimedCursedBuff : TimedBuff
    {
        private readonly BuffableEntity _be;

        public TimedCursedBuff(ScriptableBuff buff, GameObject obj, int duration) : base(buff, obj)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
        }

        protected override void ApplyEffect() { }

        protected override void End() { _be.InstantDeath(); }
    }
}