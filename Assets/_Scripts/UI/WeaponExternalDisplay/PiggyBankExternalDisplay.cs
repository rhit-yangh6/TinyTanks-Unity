using System;
using _Scripts.GameEngine;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI.WeaponExternalDisplay
{
    public class PiggyBankExternalDisplay : WeaponExternalDisplay
    {
        [SerializeField] private TextMeshProUGUI coinText;
        
        private void Start()
        {
            EventBus.AddListener(EventTypes.CoinChanged, UpdateDisplayOtherSource);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener(EventTypes.CoinChanged, UpdateDisplayOtherSource);
        }

        public override void UpdateDisplay(WeaponExtraData wed)
        {
            UpdateText();
        }

        private void UpdateDisplayOtherSource()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            coinText.text = PlayerData.Instance.coins.ToString();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            // Do nothing
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            // Do nothing
        }
    }
}