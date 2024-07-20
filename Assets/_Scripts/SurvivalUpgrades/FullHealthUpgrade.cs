using UnityEngine;

namespace _Scripts.SurvivalUpgrades
{
    [CreateAssetMenu(menuName = "_Scripts/SurvivalUpgrades/FullHealthUpgrade")]
    public class FullHealthUpgrade : SurvivalUpgrade
    {
        public override void ApplyEffect()
        {
            Debug.Log("Full Health");
        }
    }
}