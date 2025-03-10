using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

namespace _Scripts.UI.Arsenal
{
    public class ArsenalWeaponDetailPanel : MonoBehaviour
    {
        [SerializeField] private Image weaponIcon;
        [SerializeField] private GameObject infoPanel, upgradePanel, notice;
        [SerializeField] private LocalizeStringEvent weaponNameLocalizeStringEvent,
            weaponDescLocalizeStringEvent, weaponSayingLocalizeStringEvent, weaponUpgradeNameEvent,
            weaponUpgradeDescEvent, weaponUpgradePriceEvent;
        [SerializeField] private Slider[] weaponUpgradeProgressSliders;
        [SerializeField] private Button[] weaponUpgradeStarButtons;
        [SerializeField] private TextMeshProUGUI priceText, coinText;
        [SerializeField] private GameObject shopButton, setButton, shopPanel;
        
        private int _weaponId;
        
        public void SwitchDetailView(bool on = false)
        {
            infoPanel.gameObject.SetActive(on);
            upgradePanel.gameObject.SetActive(on);
            notice.gameObject.SetActive(!on);
        }
        
        public void SetDetails(int weaponId)
        {
            _weaponId = weaponId;
            SwitchDetailView(true);
            
            var w = WeaponManager.Instance.GetWeaponById(_weaponId);
            weaponIcon.sprite = w.weaponIconSprite;
            weaponNameLocalizeStringEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, w.weaponName);
            weaponDescLocalizeStringEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, w.weaponDescription);
            weaponSayingLocalizeStringEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, w.saying);
            
            // Enhanced?
            var animator = weaponIcon.GetComponent<Animator>();
            if (w.id >= 1000)
            {
                animator.runtimeAnimatorController =
                    Resources.Load<RuntimeAnimatorController>("AnimatorControllers/" + w.dataPath);
                animator.enabled = true;
            }
            else
            {
                animator.enabled = false;
            }

            for (var i = 0; i < weaponUpgradeStarButtons.Length; i++)
            {
                var tempLevel = i + 1;
                weaponUpgradeStarButtons[i].onClick.RemoveAllListeners();
                weaponUpgradeStarButtons[i].onClick.AddListener(() => StarButtonOnClick(tempLevel));
            }

            // Dealing with your current saved weapon level
            var level = PlayerData.Instance.GetWeaponLevelFromId(_weaponId);
            
            weaponUpgradeStarButtons[level - 1].Select();
            SetSliders(level);
            SetUpgradeText(level);
            
        }

        private void StarButtonOnClick(int level)
        {
            SetSliders(level);
            
            SetUpgradeText(level);
        }

        private void SetUpgradeText(int level)
        {
            int highestLevel = PlayerData.Instance.GetHighestUnlockedLevel(_weaponId);
            int currentLevel = PlayerData.Instance.GetWeaponLevelFromId(_weaponId);
            
            // Disable the Shop & Set buttons
            shopPanel.SetActive(false);
            setButton.SetActive(false);
            
            // First Level
            if (level == 1)
            {
                weaponUpgradeNameEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableUIText, 
                        Constants.LocalizationWeaponFirstLevelNameKey);
                weaponUpgradeDescEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableUIText, 
                        Constants.LocalizationWeaponFirstLevelDescKey);

                setButton.SetActive(level != currentLevel);
                
                var button = setButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SetWeaponLevel(_weaponId, level));
            } 
            // Display Proper Upgrade Details
            else if (level <= highestLevel + 1 || (highestLevel == 4 && level == 6))
            {
                Weapon w = WeaponManager.Instance.GetWeaponById(_weaponId);
                UpgradeInfo uio = w.upgradeInfos[level - 2];
                
                weaponUpgradeNameEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableWeaponUpgradeText, uio.name);
                weaponUpgradeDescEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableWeaponUpgradeText, uio.description);
                var cost = uio.cost;
                
                // Purchasable?
                if (!PlayerData.Instance.GetIfLevelUnlocked(_weaponId, level))
                {
                    shopPanel.SetActive(true);
                    weaponUpgradePriceEvent.StringReference = new LocalizedString(Constants.LocalizationTableUIText,
                        Constants.LocalizationWeaponUpgradePriceKey)
                        {{ "price", new IntVariable { Value = cost } }};
                    
                    var button = shopButton.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => BuyWeaponUpgrade(_weaponId, level));
                }
                else
                {
                    setButton.SetActive(level != currentLevel);
                    
                    var button = setButton.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => SetWeaponLevel(_weaponId, level));
                }
                
            }
            // Locked
            else if (level > highestLevel + 1)
            {
                weaponUpgradeNameEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableUIText, 
                        Constants.LocalizationWeaponLockedLevelNameKey);
                weaponUpgradeDescEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableUIText, 
                        Constants.LocalizationWeaponLockedLevelDescKey);
            }
        }
        
        private void SetSliders(int level)
        {

            foreach (Slider s in weaponUpgradeProgressSliders)
            {
                s.value = 0;
            }
            switch (level)
            {
                case 6:
                    weaponUpgradeProgressSliders[4].value = 1;
                    goto case 4;
                case 5:
                    weaponUpgradeProgressSliders[3].value = 1;
                    goto case 4;
                case 4:
                    weaponUpgradeProgressSliders[2].value = 1;
                    goto case 3;
                case 3:
                    weaponUpgradeProgressSliders[1].value = 1;
                    goto case 2;
                case 2:
                    weaponUpgradeProgressSliders[0].value = 1;
                    break;
            }
        }

        private void SetWeaponLevel(int weaponId, int levelToSet)
        {
            var result = PlayerData.Instance.SetWeaponLevel(weaponId, levelToSet);

            if (result)
            {
                setButton.SetActive(false);
            }
        }

        private void BuyWeaponUpgrade(int weaponId, int levelToBuy)
        {
            var result = PlayerData.Instance.BuyWeaponUpgrade(weaponId, levelToBuy);

            if (!result) return;
            
            SteamManager.UnlockAchievement(Constants.AchievementFirstUpgradeId);
            
            var upgradeTimes = SteamManager.IncrementStat(Constants.StatUpgradeCount);
            if (upgradeTimes >= 12)
            {
                SteamManager.UnlockAchievement(Constants.AchievementUpgrade12);
                WeaponManager.UnlockWeapon(14); // Spiky Ball 14
            }
            if (upgradeTimes >= 50)
            {
                SteamManager.UnlockAchievement(Constants.AchievementUpgrade50);
                WeaponManager.UnlockWeapon(18); // Saw Blade 18
            }
            
            if (levelToBuy > 4)
            {
                SteamManager.UnlockAchievement(Constants.AchievementMaxUpgradeId);
            }
            shopPanel.SetActive(false);
            SetWeaponLevel(weaponId, levelToBuy);
            // coinText.text = PlayerData.Instance.coins.ToString();
        }

    }
}
