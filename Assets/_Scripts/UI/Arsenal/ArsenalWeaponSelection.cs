using UnityEngine;

namespace _Scripts.UI.Arsenal
{
    public class ArsenalWeaponSelection : MonoBehaviour
    {
        [SerializeField] private DragDropIcon[] selectionIcons;

        private void OnEnable()
        {
            for (int i = 0; i < 5; i++)
            {
                selectionIcons[i].selectionIndex = i;
                if (PlayerData.Instance.selectedWeapons[i] != null)
                {
                    selectionIcons[i].SetSprite(PlayerData.Instance.selectedWeapons[i].weaponId);    

                }
            }
        }
    }
}
