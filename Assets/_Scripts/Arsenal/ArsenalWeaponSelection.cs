using System;
using UnityEngine;

namespace _Scripts
{
    public class ArsenalWeaponSelection : MonoBehaviour
    {
        [SerializeField] private DragDropIcon[] selectionIcons;

        private void OnEnable()
        {
            for (int i = 0; i < 5; i++)
            {
                selectionIcons[i].SetSprite(PlayerData.Instance.selectedWeapons[i]);
                selectionIcons[i].selectionIndex = i;
            }
        }
    }
}
