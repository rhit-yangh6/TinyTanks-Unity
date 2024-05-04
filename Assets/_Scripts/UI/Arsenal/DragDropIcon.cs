using _Scripts.GameEngine;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.UI.Arsenal
{
    public class DragDropIcon : MonoBehaviour, IPointerDownHandler,
        IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {

        [HideInInspector] public int weaponId;
        [HideInInspector] public int selectionIndex;
        
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _initPos;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            
            _initPos = _rectTransform.anchoredPosition;
        }

        public void SetSprite(int wId)
        {
            weaponId = wId;

            GetComponent<Image>().sprite = wId == 0 ? GameAssets.i.weaponLockedSprite: WeaponManager.Instance.GetWeaponById(weaponId).weaponIconSprite;
        }

        private void ResetPos()
        {
            _rectTransform.anchoredPosition = _initPos;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            ResetPos();
            // Destroy(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (weaponId != 0)
            {
                _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;    
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            DragDropGrid ddg = eventData.pointerDrag.GetComponent<DragDropGrid>();
            DragDropIcon ddi = eventData.pointerDrag.GetComponent<DragDropIcon>();
            int incomingWeaponId;

            if (ddg)
            {
                incomingWeaponId = ddg.weaponId;
                if (incomingWeaponId == 0) return;

                bool isOverrideSuccessful = PlayerData.Instance.ChangeWeaponSelection(selectionIndex, incomingWeaponId);

                if (isOverrideSuccessful)
                {
                    SetSprite(incomingWeaponId);                    
                }
            } else if (ddi)
            {
                incomingWeaponId = ddi.weaponId;
                if (incomingWeaponId == 0) return;

                bool isSwapSuccessful = PlayerData.Instance.SwapWeaponSelection(selectionIndex, ddi.selectionIndex);

                if (isSwapSuccessful)
                {
                    ddi.SetSprite(weaponId);
                    SetSprite(incomingWeaponId);
                }
            }
        }
    }
}
