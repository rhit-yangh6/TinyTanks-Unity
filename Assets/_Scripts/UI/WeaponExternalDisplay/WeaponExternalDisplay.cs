using System;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI.WeaponExternalDisplay
{
    public abstract class WeaponExternalDisplay: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private void Start()
        {
            EventBus.AddListener<WeaponExtraData>(EventTypes.ExternalDisplayChange, UpdateDisplay);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener<WeaponExtraData>(EventTypes.ExternalDisplayChange, UpdateDisplay);
        }

        public abstract void UpdateDisplay(WeaponExtraData wed);

        public abstract void OnPointerEnter(PointerEventData eventData);

        public abstract void OnPointerExit(PointerEventData eventData);
    }
}