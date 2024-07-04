using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace _Scripts.UI.WeaponExternalDisplay
{
    public class PuzzleExternalDisplay: WeaponExternalDisplay
    {
        [SerializeField] private CanvasGroup[] pieces;
        
        private PuzzleExtraData _puzzleExtraData;
        private void Start()
        {
            EventBus.AddListener<PuzzleExtraData>(EventTypes.ExternalDisplayChange, UpdateDisplay);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener<PuzzleExtraData>(EventTypes.ExternalDisplayChange, UpdateDisplay);
        }
        
        public override void UpdateDisplay(WeaponExtraData wed)
        {
            _puzzleExtraData = (PuzzleExtraData) wed;
            var status = _puzzleExtraData.GetStatus();
            for (var i = 0; i < pieces.Length; i++)
            {
                pieces[i].alpha = status.Contains(i) ? 1 : 0.2f;
            }
        }

        public override async void OnPointerEnter(PointerEventData eventData)
        {
            var nameOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableWeaponExtraDisplayText, 
                    "puzzle_extra_display_name");
            var descOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableWeaponExtraDisplayText,
                    "puzzle_extra_display_desc");
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