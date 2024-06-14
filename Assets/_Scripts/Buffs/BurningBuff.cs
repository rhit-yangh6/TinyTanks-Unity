using _Scripts.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Buffs
{
    [CreateAssetMenu(menuName = "_Scripts/Buffs/BurningBuff")]
    public class BurningBuff: ScriptableBuff
    {
        public float burningDamage;
        
        public override TimedBuff InitializeBuff(GameObject obj, int level = 1)
        {
            return new TimedBurningBuff(this, obj, level == 4 ? 999 : duration, level);
        }
    }
    
    public class TimedBurningBuff : TimedBuff
    {
        private readonly BuffableEntity _be;
        private readonly float _finalBurningDamage;

        public TimedBurningBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            //Getting MovementComponent, replace with your own implementation
            _be = obj.GetComponent<BuffableEntity>();
            var burningBuff = (BurningBuff)Buff;
            _finalBurningDamage = level switch
            {
                >= 3 => burningBuff.burningDamage * 4,
                2 => burningBuff.burningDamage * 2,
                1 => burningBuff.burningDamage,
                _ => _finalBurningDamage
            };
        }

        protected override void ApplyEffect() { }

        protected override void End() { }

        protected override void TurnTrigger()
        {
            var oilyBuff = _be.GetBuff("OilyBuff");
            Debug.Log(oilyBuff);
            if (oilyBuff != null)
            {
                _be.TakeDamage(_finalBurningDamage * ((OilyBuff)oilyBuff).burnDamageMultiplier);
            }
            else
            {
                _be.TakeDamage(_finalBurningDamage);
            }
        }
    }
}