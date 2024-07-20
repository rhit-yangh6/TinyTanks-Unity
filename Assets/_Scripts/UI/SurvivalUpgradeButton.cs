using _Scripts.SurvivalUpgrades;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class SurvivalUpgradeButton : MonoBehaviour
    {
        [SerializeField] private LocalizeStringEvent upgradeNameEvent;
        [SerializeField] private LocalizeStringEvent upgradeDescEvent;
        [SerializeField] private Image upgradeIcon;

        public void UpdateDisplay(SurvivalUpgrade survivalUpgrade)
        {
            upgradeNameEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, survivalUpgrade.upgradeName);
            upgradeDescEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, survivalUpgrade.desc);
            upgradeIcon.sprite = survivalUpgrade.icon;
        }
    }
}