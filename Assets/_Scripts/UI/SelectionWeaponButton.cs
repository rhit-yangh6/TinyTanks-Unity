using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI
{
    public class SelectionWeaponButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector] public int weaponId;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.ShowTooltip_Static(weaponId);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.HideTooltip_Static();
        }
    }
}
