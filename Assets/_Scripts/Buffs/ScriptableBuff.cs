using UnityEngine;

namespace _Scripts.Buffs
{
    public abstract class ScriptableBuff : ScriptableObject
    {
        /**
        * Time duration of the buff in turns.
        */
        public int duration;

        /**
        * Duration is increased each time the buff is applied.
        */
        public bool isDurationStacked;

        /**
        * Effect value is increased each time the buff is applied.
        */
        public bool isEffectStacked;

        public abstract TimedBuff InitializeBuff(GameObject obj);
    }
    
    public abstract class TimedBuff
    {
        protected int duration;
        protected int effectStacks;
        public ScriptableBuff Buff { get; }
        protected readonly GameObject obj;
        public bool isFinished;

        public TimedBuff(ScriptableBuff buff, GameObject obj)
        {
            Buff = buff;
            this.obj = obj;
        }

        public void Tick()
        {
            duration -= 1;
            if (duration < 0)
            {
                End();
                isFinished = true;
            }
        }

        /**
        * Activates buff or extends duration if ScriptableBuff has IsDurationStacked or IsEffectStacked set to true.
        */
        public void Activate()
        {
            if (Buff.isEffectStacked || duration <= 0)
            {
                ApplyEffect();
                effectStacks++;
            }
        
            if (Buff.isDurationStacked || duration <= 0)
            {
                duration += Buff.duration;
            }
        }
        protected abstract void ApplyEffect();
        public abstract void End();
    }
}
