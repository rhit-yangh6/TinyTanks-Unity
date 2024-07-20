using UnityEngine;

namespace _Scripts.SurvivalUpgrades
{
    [CreateAssetMenu(menuName = "_Scripts/SurvivalUpgrades/RestoreHealthUpgrade")]
    public class RestoreHealthUpgrade : SurvivalUpgrade
    {
        public override void ApplyEffect()
        {
            Debug.Log("Restore Health");
        }
    }
}