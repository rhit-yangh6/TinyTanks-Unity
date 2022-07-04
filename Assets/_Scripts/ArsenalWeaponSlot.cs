using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts
{
    public class ArsenalWeaponSlot : MonoBehaviour, IDropHandler
    {

        // TODO: Pre-populate
        [SerializeField] public int weaponSlotIndex;
        [SerializeField] public GameObject ddiPrefab;
        private Transform _wst;

        private void Awake()
        {
            _wst = GameObject.Find("WeaponSelectionTab").transform;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("OnDrop");
            DragDropGrid ddg = eventData.pointerDrag.GetComponent<DragDropGrid>();
            DragDropIcon ddi = eventData.pointerDrag.GetComponent<DragDropIcon>();
            if (ddg || ddi)
            {
                int weaponId = ddg ? ddg.weaponId : eventData.pointerDrag.GetComponent<DragDropIcon>().weaponId;
                
                GameObject ddiObject = Instantiate(ddiPrefab, transform.position, Quaternion.identity, _wst);
                DragDropIcon dragDropIcon = ddiObject.GetComponent<DragDropIcon>();
                
                dragDropIcon.weaponId = weaponId;
                dragDropIcon.SetSprite();
                dragDropIcon.selectionIndex = weaponSlotIndex;
                
                // TODO: Save?
            }
        }
    }
}
