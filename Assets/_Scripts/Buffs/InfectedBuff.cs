using _Scripts.Entities;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/InfectedBuff")]
    public class InfectedBuff: ScriptableBuff
    {
        public float infectedDamage;
        
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedInfectedBuff(this, obj, duration, level);
        }
    }
    
    public class TimedInfectedBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private readonly float _finalInfectedDamage;

        public TimedInfectedBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
            var infectedBuff = (InfectedBuff) Buff;
            _finalInfectedDamage = level switch
            {
                >= 3 => infectedBuff.infectedDamage * 4,
                2 => infectedBuff.infectedDamage * 2,
                1 => infectedBuff.infectedDamage,
                _ => _finalInfectedDamage
            };
        }

        protected override void ApplyEffect() { }

        protected override void End()
        {
            // Spread
            DamageHandler.i.HandleDamage(_be.transform.position, 
                10, 0, DamageHandler.DamageType.Circular, false,
                GameAssets.i.infectedBuff, 1);
        }

        protected override void TurnTrigger()
        {
            _be.TakeDamage(_finalInfectedDamage);
        }
    }
}