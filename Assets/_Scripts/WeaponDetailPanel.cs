using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class WeaponDetailPanel : MonoBehaviour
    {

        [SerializeField] private Image weaponIcon;
        [SerializeField] private GameObject infoPanel, upgradePanel, notice;
        [SerializeField] private TextMeshProUGUI weaponNameText, weaponDescText;
        [SerializeField] private Slider[] weaponUpgradeProgressSliders;
        [SerializeField] private Button[] weaponUpgradeStarButtons;
        [SerializeField] private TextMeshProUGUI upgradeNameText, upgradeDescText, priceText, coinText;
        [SerializeField] private GameObject shopButton, setButton, shopPanel;
        
        private int _weaponId;
        private const string FirstLevelUpgradeName = "Basic Level";
        private const string FirstLevelUpgradeDesc = "";
        private const string LockedLevelUpgradeName = "Locked";
        private const string LockedLevelUpgradeDesc = "Unlock previous levels first!";
        
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
            
            Weapon w = WeaponManager.Instance.GetWeaponById(_weaponId);
            weaponIcon.sprite = w.weaponIconSprite;
            weaponNameText.text = w.weaponName;
            weaponDescText.text = w.weaponDescription;

            for (int i = 0; i < weaponUpgradeStarButtons.Length; i++)
            {
                int tempLevel = i + 1;
                weaponUpgradeStarButtons[i].onClick.RemoveAllListeners();
                weaponUpgradeStarButtons[i].onClick.AddListener(() => StarButtonOnClick(tempLevel));
            }

            // Dealing with your current saved weapon level
            int level = PlayerData.Instance.GetWeaponLevelFromId(_weaponId);
            
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
                upgradeNameText.text = FirstLevelUpgradeName;
                upgradeDescText.text = FirstLevelUpgradeDesc;

                setButton.SetActive(level != currentLevel);
            } 
            // Display Proper Upgrade Details
            else if (level <= highestLevel + 1 || (highestLevel == 4 && level == 6))
            {
                Weapon w = WeaponManager.Instance.GetWeaponById(_weaponId);
                UpgradeInfo uio = w.upgradeInfos[level - 2];
                
                upgradeNameText.text = uio.name;
                upgradeDescText.text = uio.description;
                int cost = uio.cost;
                
                // Purchasable?
                if (!PlayerData.Instance.GetIfLevelUnlocked(_weaponId, level))
                {
                    shopPanel.SetActive(true);
                    priceText.text = "Price: " + cost;
                    
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
                upgradeNameText.text = LockedLevelUpgradeName;
                upgradeDescText.text = LockedLevelUpgradeDesc;
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
            
            shopPanel.SetActive(false);
            SetWeaponLevel(weaponId, levelToBuy);
        }

    }
}
