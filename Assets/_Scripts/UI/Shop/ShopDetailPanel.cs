using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace _Scripts.UI.Shop
{
    public class ShopDetailPanel : MonoBehaviour
    {
        [SerializeField] private Image weaponIcon;
        [SerializeField] private GameObject notice, infoPanel, purchasePanel;
        [SerializeField] private TextMeshProUGUI priceText, coinText;
        [SerializeField] private GameObject shopButton, newWeaponMsg;
        [SerializeField] private LocalizeStringEvent weaponNameEvent, weaponDescEvent;
        
        private int _weaponId;

        private void OnEnable()
        {
            SwitchDetailView();
        }

        private void SwitchDetailView(bool on = false)
        {
            infoPanel.gameObject.SetActive(on);
            notice.gameObject.SetActive(!on);
        }

        public void SetDetails(int weaponId)
        {
            _weaponId = weaponId;
            SwitchDetailView(true);
            
            var w = WeaponManager.Instance.GetWeaponById(_weaponId);
            weaponIcon.sprite = w.weaponIconSprite;
            weaponNameEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, w.weaponName);
            weaponDescEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, w.weaponDescription);

            if (PlayerData.Instance.GetWeaponLevelFromId(_weaponId) > 0)
            {
                purchasePanel.SetActive(false);
                return;
            }
            
            purchasePanel.SetActive(true);
            priceText.text = w.shopPrice.ToString();

            shopButton.SetActive(true);
            var button = shopButton.GetComponent<Button>();
            button.onClick.RemoveAllListeners();

            var wId = _weaponId;
            button.onClick.AddListener(() => BuyWeapon(wId));

        }

        private void BuyWeapon(int weaponId)
        {
            var result = WeaponManager.Instance.UnlockWeapon(weaponId);
            if (!result) return;
            purchasePanel.SetActive(false);
            coinText.text = PlayerData.Instance.coins.ToString();
        }
    }
}
