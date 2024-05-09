using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExplosionGame
{
    public class WeaponSelection : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Image _selectionImage;

        public Weapon Weapon => _weapon;
        private Action<WeaponSelection> _onSelectedCallback;

        public void Compose(Action<WeaponSelection> callback)
        {
            _onSelectedCallback = callback;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onSelectedCallback(this);
        }

        public void Select()
        {
            _selectionImage.gameObject.SetActive(true);
        }

        public void Unselect()
        {
            _selectionImage.gameObject.SetActive(false);
        }
    }
}