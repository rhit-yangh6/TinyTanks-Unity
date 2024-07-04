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
        
        private TextMeshProUGUI _tooltipDescText;
        private TextMeshProUGUI _tooltipNameText;
        private RectTransform _bgRectTransform;
        private RectTransform _mainRectTransform;
        private Canvas _canvas;

        private const float TextPaddingSize = 6f;

        private void Awake()
        {
            _instance = this;
            _mainRectTransform = GetComponent<RectTransform>();
            _bgRectTransform = transform.Find("BG").GetComponent<RectTransform>();
            _tooltipDescText = transform.Find("DescText").GetComponent<TextMeshProUGUI>();
            _tooltipNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            _canvas = GameObject.FindWithTag("UI").GetComponent<Canvas>();
        }

        private void Update()
        {
            var screenPoint = Input.mousePosition / _canvas.scaleFactor;
            _mainRectTransform.anchoredPosition =
                screenPoint + new Vector3(tooltipMouseDistanceX,tooltipMouseDistanceY, 0);
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
                _tooltipNameText.text = nameOperationAsync.Result;
                _tooltipDescText.text = descOperationAsync.Result;
            }
            // tooltipDescLocalizeStringEvent.StringReference =
            //     new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponDescription);
            // tooltipNameLocalizeStringEvent.StringReference =
            //     new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponName);
            
            var backgroundSize = new Vector2(_tooltipDescText.preferredWidth + TextPaddingSize * 2f,
                _tooltipNameText.preferredHeight + TextPaddingSize * 3f + _tooltipDescText.preferredHeight);
            
            _tooltipNameText.rectTransform.anchoredPosition =
                new Vector2(TextPaddingSize, _tooltipDescText.preferredHeight + TextPaddingSize * 2f);
            
            _bgRectTransform.sizeDelta = backgroundSize;
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
                _tooltipNameText.text = nameOperationAsync.Result;
                _tooltipDescText.text = descOperationAsync.Result;
            }
            
            var backgroundSize = new Vector2(_tooltipDescText.preferredWidth + TextPaddingSize * 2f,
                _tooltipNameText.preferredHeight + TextPaddingSize * 3f + _tooltipDescText.preferredHeight);
            
            _tooltipNameText.rectTransform.anchoredPosition =
                new Vector2(TextPaddingSize, _tooltipDescText.preferredHeight + TextPaddingSize * 2f);
            
            _bgRectTransform.sizeDelta = backgroundSize;
        }

        private void ShowTooltipString(string displayName, string displayDesc)
        {
            gameObject.SetActive(true);
            
            _tooltipNameText.text = displayName;
            _tooltipDescText.text = displayDesc; 
            var backgroundSize = new Vector2(_tooltipDescText.preferredWidth + TextPaddingSize * 2f,
                _tooltipNameText.preferredHeight + TextPaddingSize * 3f + _tooltipDescText.preferredHeight);
            
            _tooltipNameText.rectTransform.anchoredPosition =
                new Vector2(TextPaddingSize, _tooltipDescText.preferredHeight + TextPaddingSize * 2f);
            
            _bgRectTransform.sizeDelta = backgroundSize;
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

        public static void ShowTooltipString_Static(string displayName, string displayDesc)
        {
            _instance.ShowTooltipString(displayName, displayDesc);
        }
        
        public static void HideTooltip_Static()
        {
            _instance.HideTooltip();
        }
    }
}
