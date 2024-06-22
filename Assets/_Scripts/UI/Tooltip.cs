using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace _Scripts.UI
{
    public class Tooltip : MonoBehaviour
    {
        private static Tooltip _instance;

        [SerializeField] private float tooltipMouseDistanceX = 100f;
        [SerializeField] private float tooltipMouseDistanceY = 3f;
        
        private TextMeshProUGUI tooltipDescText;
        private TextMeshProUGUI tooltipNameText;
        private RectTransform bgRectTransform;
        private RectTransform mainRectTransform;

        private const float TextPaddingSize = 6f;

        private void Awake()
        {
            _instance = this;
            mainRectTransform = GetComponent<RectTransform>();
            bgRectTransform = transform.Find("BG").GetComponent<RectTransform>();
            tooltipDescText = transform.Find("DescText").GetComponent<TextMeshProUGUI>();
            tooltipNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            mainRectTransform.anchoredPosition =
                Input.mousePosition + new Vector3(tooltipMouseDistanceX,tooltipMouseDistanceY, 0);
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
