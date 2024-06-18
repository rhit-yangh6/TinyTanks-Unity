using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI
{
    public class BuffDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector] public string buffKey;
        [HideInInspector] public int buffDuration;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.ShowTooltipBuff_Static(buffKey, buffDuration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.HideTooltip_Static();
        }
    }
}
