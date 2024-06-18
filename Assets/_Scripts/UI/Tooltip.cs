using System;
using _Scripts.Managers;
using _Scripts.Projectiles;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
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
            tooltipDescText = transform.Find("DescText").GetComponent<TextMeshProUGUI>();
            tooltipNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            tooltipDescLocalizeStringEvent = transform.Find("DescText").GetComponent<LocalizeStringEvent>();
            tooltipNameLocalizeStringEvent = transform.Find("NameText").GetComponent<LocalizeStringEvent>();
        }

        private void Update()
        {
            mainRectTransform.anchoredPosition =
                Input.mousePosition + new Vector3(tooltipMouseDistance,tooltipMouseDistance, 0);
        }

        private void ShowTooltipWeapon(int weaponId)
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
        
        private void ShowTooltipBuff(string buffKey, int duration)
        {
            // Plus 1 to the duration
            duration += 1;
            Debug.Log(buffKey + " " + duration);
            gameObject.SetActive(true);
            
            tooltipDescLocalizeStringEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableBuffText, "buff_" + buffKey + "_desc")
                    {{ "duration", new IntVariable { Value = duration } }};
            tooltipNameLocalizeStringEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableBuffText, "buff_" + buffKey + "_name");
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

        public static void ShowTooltipWeapon_Static(int weaponId)
        {
            _instance.ShowTooltipWeapon(weaponId);
        }
        
        public static void ShowTooltipBuff_Static(string buffKey, int duration)
        {
            _instance.ShowTooltipBuff(buffKey, duration);
        }
        
        public static void HideTooltip_Static()
        {
            _instance.HideTooltip();
        }
    }
}
