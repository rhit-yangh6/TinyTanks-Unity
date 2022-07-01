using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class WeaponDetailPanel : MonoBehaviour
    {

        public Image weaponIcon;
        public GameObject infoPanel, upgradePanel, notice;
        public TextMeshProUGUI weaponNameText, weaponDescText;
        public Slider[] weaponUpgradeProgressSliders;
        
        public void SetDetails(int weaponId)
        {
            SwitchDetailView(true);
            
            Weapon w = WeaponManager.Instance.GetWeaponById(weaponId);
            weaponIcon.sprite = w.weaponIconSprite;
            weaponNameText.text = w.weaponName;
            weaponDescText.text = w.weaponDescription;
            SetSliders(weaponId);
        }

        public void SwitchDetailView(bool on = false)
        {
            infoPanel.gameObject.SetActive(on);
            upgradePanel.gameObject.SetActive(on);
            notice.gameObject.SetActive(!on);
        }

        private void SetSliders(int weaponId)
        {
            int level = PlayerData.Instance.GetWeaponLevelFromId(weaponId);

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
    }
}
