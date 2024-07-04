using System;
using System.Collections.Generic;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

namespace _Scripts.UI.WeaponExternalDisplay
{
    public class GearExternalDisplay: WeaponExternalDisplay
    {
        [SerializeField] private TextMeshProUGUI gearNumberText;

        private GearExtraData _gearExtraData;
        
        private void Start()
        {
            EventBus.AddListener<GearExtraData>(EventTypes.ExternalDisplayChange, UpdateDisplay);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener<GearExtraData>(EventTypes.ExternalDisplayChange, UpdateDisplay);
        }

        public override void UpdateDisplay(WeaponExtraData wed)
        {
            _gearExtraData = (GearExtraData) wed;
            gearNumberText.text = _gearExtraData.gearNum.ToString();
        }

        public override async void OnPointerEnter(PointerEventData eventData)
        {
            var nameOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableWeaponExtraDisplayText, 
                    "gear_extra_display_name");
            var descOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableWeaponExtraDisplayText,
                    "gear_extra_display_desc",
                    new object[] {new Dictionary<string, string>
                        {{ "count", _gearExtraData.gearNum.ToString() }}});
            await nameOperationAsync.Task;
            await descOperationAsync.Task;
            
            if (nameOperationAsync.IsDone && descOperationAsync.IsDone)
            {
                Tooltip.ShowTooltipString_Static(nameOperationAsync.Result, descOperationAsync.Result);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.HideTooltip_Static();
        }
    }
}