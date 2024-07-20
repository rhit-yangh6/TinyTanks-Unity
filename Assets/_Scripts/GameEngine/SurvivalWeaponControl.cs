using System.Collections.Generic;
using System.Linq;
using _Scripts.Managers;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.GameEngine
{
    public class SurvivalWeaponControl : WeaponControl
    {
        protected override void GetSelectedWeapons()
        {
            SelectedWeapons = new SelectionDatum[]
            {
                new (1, 1), // Boulder
                new (13, 1), // Med Kit
                new (-1, 6),
                null,
                null
            };
        }

        public bool HasEmptySlot()
        {
            return SelectedWeapons.Any(datum => datum == null);
        }

        public void AddWeapon(int weaponId)
        {
            for (var i = 0; i < SelectedWeapons.Length; i++)
            {
                if (SelectedWeapons[i] != null) continue;
                
                SelectedWeapons[i] = new SelectionDatum(weaponId, 1);
                break;
            }
            RefreshDisplay();
        }

        public void SwitchWeapon(int index, int weaponId)
        {
            SelectedWeapons[index] = new SelectionDatum(weaponId, 1);
            RefreshDisplay();
        }

        public void UpgradeWeapon(int index, bool isUpperPath = true)
        {
            var datum = SelectedWeapons[index];
            switch (datum.level)
            {
                case 6:
                    datum.level = 5;
                    break;
                case 5:
                    datum.level = 6;
                    break;
                case 4:
                    datum.level = isUpperPath ? 5 : 6;
                    break;
                default:
                    datum.level++;
                    break;
            }
        }
    }
}
