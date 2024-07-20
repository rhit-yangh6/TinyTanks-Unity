using UnityEngine;

namespace _Scripts.SurvivalUpgrades
{
    public abstract class SurvivalUpgrade : ScriptableObject
    {
        public string upgradeName;
        public string desc;
        public Sprite icon;
        
        public abstract void ApplyEffect();
    }
}