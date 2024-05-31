using System;
using _Scripts.Managers;
using _Scripts.Projectiles;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Random = System.Random;

namespace _Scripts.UI
{
    public class Tooltip : MonoBehaviour
    {
        private static Tooltip _instance;

        [SerializeField] private float tooltipMouseDistance = 3f;
        
        private TextMeshProUGUI tooltipDescText;
        private TextMeshProUGUI tooltipNameText;
        private LocalizeStringEvent tooltipDescLocalizeStringEvent;
        private LocalizeStringEvent tooltipNameLocalizeStringEvent;
        private RectTransform bgRectTransform;
        private RectTransform mainRectTransform;

        private void Awake()
        {
            _instance = this;
            mainRectTransform = GetComponent<RectTransform>();
            bgRectTransform = transform.Find("BG").GetComponent<RectTransform>();
            tooltipDescText = transform.Find("WeaponDescText").GetComponent<TextMeshProUGUI>();
            tooltipNameText = transform.Find("WeaponNameText").GetComponent<TextMeshProUGUI>();
            tooltipDescLocalizeStringEvent = transform.Find("WeaponDescText").GetComponent<LocalizeStringEvent>();
            tooltipNameLocalizeStringEvent = transform.Find("WeaponNameText").GetComponent<LocalizeStringEvent>();
        }

        private void Update()
        {
            mainRectTransform.anchoredPosition =
                Input.mousePosition + new Vector3(tooltipMouseDistance,tooltipMouseDistance, 0);
        }

        private void ShowTooltip(int weaponId)
        {
            if (weaponId == 0) return;
            gameObject.SetActive(true);

            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            tooltipDescLocalizeStringEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponDescription);
            tooltipNameLocalizeStringEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponName);
            const float textPaddingSize = 4f;
            
            var backgroundSize = new Vector2(tooltipDescText.preferredWidth + textPaddingSize * 2f,
                tooltipNameText.preferredHeight + textPaddingSize * 3f + tooltipDescText.preferredHeight);
            
            tooltipNameText.rectTransform.anchoredPosition =
                new Vector2(textPaddingSize, tooltipDescText.preferredHeight + textPaddingSize * 2f);
            
            bgRectTransform.sizeDelta = backgroundSize;
        }

        private void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        public static void ShowTooltip_Static(int weaponId)
        {
            _instance.ShowTooltip(weaponId);
        }
        
        public static void HideTooltip_Static()
        {
            _instance.HideTooltip();
        }
    }
}
