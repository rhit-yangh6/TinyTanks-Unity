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
        [SerializeField] private TextMeshProUGUI priceText, coinText;
        [SerializeField] private Button cancelButton, shopButton;
        [SerializeField] private GameObject blurPanel;
        [SerializeField] private LocalizeStringEvent weaponNameEvent, weaponDescEvent;
        
        private int _weaponId;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cancelButton.onClick.Invoke();
            }
        }

        public void SetDetails(int weaponId)
        {
            _weaponId = weaponId;
            
            var w = WeaponManager.Instance.GetWeaponById(_weaponId);
            weaponIcon.sprite = w.weaponIconSprite;
            weaponNameEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, w.weaponName);
            weaponDescEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, w.weaponDescription);
            
            priceText.text = w.shopPrice.ToString();

            shopButton.onClick.RemoveAllListeners();

            var wId = _weaponId;
            shopButton.onClick.AddListener(() => BuyWeapon(wId));

        }

        private void BuyWeapon(int weaponId)
        {
            var result = WeaponManager.UnlockWeapon(weaponId);
            if (!result) return;
            coinText.text = PlayerData.Instance.coins.ToString();
            blurPanel.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
