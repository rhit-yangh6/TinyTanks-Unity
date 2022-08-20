using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/CursedBuff")]
    public class CursedBuff: ScriptableBuff
    {
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedCursedBuff(this, obj, level == 2 ? duration - 1 : duration, level);
        }
    }
    
    public class TimedCursedBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private readonly bool _willExplode;

        public TimedCursedBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
            _willExplode = level >= 3;
        }

        protected override void ApplyEffect() { }

        protected override void End()
        {
            if (_willExplode)
            {
                _be.SelfExplode();
            }
            _be.InstantDeath();
        }
    }
}