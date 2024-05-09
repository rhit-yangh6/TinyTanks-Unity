using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DemosShared
{
    public class InteractiveUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsTouching { get; private set; }

        public event Action PointerDown;
        public event Action PointerUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            IsTouching = true;
            PointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsTouching = false;
            PointerUp?.Invoke();
        }
    }
}