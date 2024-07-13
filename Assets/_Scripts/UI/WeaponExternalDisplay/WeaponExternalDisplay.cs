using System;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI.WeaponExternalDisplay
{
    public abstract class WeaponExternalDisplay: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public abstract void UpdateDisplay(WeaponExtraData wed);

        public abstract void OnPointerEnter(PointerEventData eventData);

        public abstract void OnPointerExit(PointerEventData eventData);
    }
}