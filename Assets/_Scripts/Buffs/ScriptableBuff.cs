using System;
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
        
        /**
        * Trigger on turn starts
        */
        public bool isTurnTriggered;

        /**
        * Icon of the buff
        */
        public Sprite buffIcon;
        
        /**
        * Key of the buff
        */
        public string buffKey;

        public abstract TimedBuff InitializeBuff(GameObject obj, int level = 1);
    }
    
    public abstract class TimedBuff
    {
        public int duration;
        protected int effectStacks;
        public ScriptableBuff Buff { get; }
        protected readonly GameObject obj;
        public bool isFinished;
        protected int InitialDuration;

        protected TimedBuff(ScriptableBuff buff, GameObject obj, int duration)
        {
            Buff = buff;
            this.obj = obj;
            InitialDuration = duration;
        }

        public void Tick()
        {
            duration -= 1;
            // Debug.Log(duration);
            if (duration < 0)
            {
                End();
                isFinished = true;
            }
            else
            {
                if (Buff.isTurnTriggered)
                {
                    TurnTrigger();
                }
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
                duration += InitialDuration;
            }
        }
        protected abstract void ApplyEffect();
        protected abstract void End();
        protected virtual void TurnTrigger() { /* Do Nothing */ }
    }
}
