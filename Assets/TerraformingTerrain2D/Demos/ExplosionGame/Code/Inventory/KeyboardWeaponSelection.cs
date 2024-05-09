using UnityEngine;

namespace ExplosionGame
{
    public class KeyboardWeaponSelection : MonoBehaviour
    {
        [SerializeField] private WeaponSelection[] _weaponSelections;
        [SerializeField] private Inventory _inventory;
        private readonly KeyCode[] _keys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

        private void Update()
        {
            for (int i = 0; i < _keys.Length; ++i)
            {
                if (Input.GetKeyDown(_keys[i]))
                {
                    _inventory.SelectWeapon(_weaponSelections[i]);
                }
            }
        }
    }
}