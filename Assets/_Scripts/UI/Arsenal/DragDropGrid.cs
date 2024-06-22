using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI.Arsenal
{
    public class DragDropGrid : MonoBehaviour, IPointerDownHandler,
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public GameObject draggableIconPrefab;
        [HideInInspector] public int weaponId;
        
        private GameObject _clone;

        private RectTransform _cloneRectTransform, _rectTransform;
        private CanvasGroup _canvasGroup;
        private Transform _wst;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _wst = GameObject.Find("WeaponSelectionTab").transform;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (weaponId == 0) return;
            
            var screenPoint = Input.mousePosition;
            screenPoint.z = 10.0f; //distance of the plane from the camera
            if (Camera.main != null)
                _clone = Instantiate(draggableIconPrefab, Camera.main.ScreenToWorldPoint(screenPoint),
                    Quaternion.identity);
            _clone.transform.SetParent (GameObject.FindGameObjectWithTag("UI").transform, false);
            _cloneRectTransform = _clone.GetComponent<RectTransform>();
            _cloneRectTransform.sizeDelta = _rectTransform.sizeDelta;
            _canvasGroup = _clone.GetComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = false;

            DragDropIcon ddi = _clone.GetComponent<DragDropIcon>();
            ddi.SetSprite(weaponId);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (weaponId == 0) return;
            
            _canvasGroup.blocksRaycasts = true;
            Destroy(_clone);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (weaponId == 0) return;

            var screenPoint = Input.mousePosition;
            screenPoint.z = 10.0f; //distance of the plane from the camera
            if (Camera.main != null) _cloneRectTransform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        }
        
    }
}
