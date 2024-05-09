using UnityEngine;

namespace ExplosionGame
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private WeaponSelection[] _weaponSelections;
        [SerializeField] private AudioSource _itemEquipped;
        private WeaponSelection _selection;
        private Unit _unit;

        public void Compose(Unit unit, Weapon initialWeapon)
        {
            _unit = unit;

            foreach (WeaponSelection weaponSelection in _weaponSelections)
            {
                weaponSelection.Compose(SelectWeapon);

                if (weaponSelection.Weapon == initialWeapon)
                {
                    SwitchSelection(weaponSelection);
                }
            }
        }

        public void SelectWeapon(WeaponSelection weaponSelection)
        {
            SwitchSelection(weaponSelection);
            _unit.WeaponHolder.SetWeapon(weaponSelection.Weapon);
            _itemEquipped.Play();
        }

        private void SwitchSelection(WeaponSelection weaponSelection)
        {
            if (_selection != null)
                _selection.Unselect();

            _selection = weaponSelection;

            _selection.Select();
        }
    }
}