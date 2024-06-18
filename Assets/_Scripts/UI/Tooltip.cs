using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Projectiles;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.ResourceManagement.AsyncOperations;
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

        private const float TextPaddingSize = 4f;

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

        private async void ShowTooltipWeapon(int weaponId)
        {
            if (weaponId == 0) return;
            gameObject.SetActive(true);

            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            
            var nameOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableWeaponText, weapon.weaponName);
            var descOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableWeaponText, weapon.weaponDescription);
            await nameOperationAsync.Task;
            await descOperationAsync.Task;
            
            if (nameOperationAsync.IsDone && descOperationAsync.IsDone)
            {
                tooltipNameText.text = nameOperationAsync.Result;
                tooltipDescText.text = descOperationAsync.Result;
            }
            // tooltipDescLocalizeStringEvent.StringReference =
            //     new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponDescription);
            // tooltipNameLocalizeStringEvent.StringReference =
            //     new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponName);
            
            var backgroundSize = new Vector2(tooltipDescText.preferredWidth + TextPaddingSize * 2f,
                tooltipNameText.preferredHeight + TextPaddingSize * 3f + tooltipDescText.preferredHeight);
            
            tooltipNameText.rectTransform.anchoredPosition =
                new Vector2(TextPaddingSize, tooltipDescText.preferredHeight + TextPaddingSize * 2f);
            
            bgRectTransform.sizeDelta = backgroundSize;
        }
        
        private async void ShowTooltipBuff(string buffKey, int duration)
        {
            // Plus 1 to the duration
            duration += 1;
            gameObject.SetActive(true);
            
            var nameOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableBuffText, "buff_" + buffKey + "_name");
            var descOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableBuffText, "buff_" + buffKey + "_desc",
                    new object[] {new Dictionary<string, string> {{ "duration", duration.ToString() }}});
            await nameOperationAsync.Task;
            await descOperationAsync.Task;
            // tooltipDescLocalizeStringEvent.StringReference =
            //     new LocalizedString(Constants.LocalizationTableBuffText, "buff_" + buffKey + "_desc")
            //         {{ "duration", new IntVariable { Value = duration } }};
            // tooltipNameLocalizeStringEvent.StringReference =
            //     new LocalizedString(Constants.LocalizationTableBuffText, "buff_" + buffKey + "_name");
            
            if (nameOperationAsync.IsDone && descOperationAsync.IsDone)
            {
                tooltipNameText.text = nameOperationAsync.Result;
                tooltipDescText.text = descOperationAsync.Result;
            }
            
            var backgroundSize = new Vector2(tooltipDescText.preferredWidth + TextPaddingSize * 2f,
                tooltipNameText.preferredHeight + TextPaddingSize * 3f + tooltipDescText.preferredHeight);
            
            tooltipNameText.rectTransform.anchoredPosition =
                new Vector2(TextPaddingSize, tooltipDescText.preferredHeight + TextPaddingSize * 2f);
            
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
