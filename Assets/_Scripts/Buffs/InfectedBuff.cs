using _Scripts.Entities;
using _Scripts.Managers;
using Unity.VisualScripting;
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
        private int _level;
        private float _baseRadius = 6f;

        public TimedInfectedBuff(ScriptableBuff buff, GameObject obj, int duration, int level) : base(buff, obj, duration)
        {
            _level = level;
            _be = obj.GetComponent<BuffableEntity>();
            var infectedBuff = (InfectedBuff) Buff;
            _finalInfectedDamage = _level switch
            {
                >= 3 => infectedBuff.infectedDamage * 3,
                >= 1 => infectedBuff.infectedDamage,
                _ => _finalInfectedDamage
            };
        }

        protected override void ApplyEffect() { }

        protected override void End()
        {

            var finalRadius = _level switch
            {
                4 => _baseRadius * 3f,
                >= 2 => _baseRadius * 1.5f,
                _ => _baseRadius
            };
            // Spread
            DamageHandler.i.HandleDamage(_be.transform.position, 
                finalRadius, 0, DamageHandler.DamageType.Circular, false,
                GameAssets.i.infectedBuff, _level);
        }

        protected override void TurnTrigger()
        {
            // Chance to spread
            if (_level == 4)
            {
                // 30% chance to spread
                if (Random.value > 0.7)
                {
                    // Spread
                    DamageHandler.i.HandleDamage(_be.transform.position, 
                        _baseRadius * 3f, 0, DamageHandler.DamageType.Circular, false,
                        GameAssets.i.infectedBuff, _level);
                }
            }
            // Chance to kill
            if (_level == 5)
            {
                // 6% chance to instant kill
                if (Random.value > 0.94)
                {
                    _be.InstantDeath();
                    return;
                }
            }
            
            _be.TakeDamage(_finalInfectedDamage);
        }
    }
}