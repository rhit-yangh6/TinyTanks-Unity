using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts
{
    public class DragDropIcon : MonoBehaviour, IPointerDownHandler,
        IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {

        [HideInInspector] public int weaponId;
        [HideInInspector] public int selectionIndex;
        
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }

        public void SetSprite()
        {
            GetComponent<Image>().sprite = WeaponManager.Instance.GetWeaponById(weaponId).weaponIconSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Destroy(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnDrop(PointerEventData eventData)
        {
            // TODO: Swap?
        }
    }
}
