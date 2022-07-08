using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class WeaponControl : MonoBehaviour
    {

        public LaunchProjectile lp;
        public GameObject buttonPrefab;
        public Sprite weaponLockedSprite;
        private readonly List<Button> _weaponButtons = new ();
        private int _selectedIdx = 0;

        private void Start()
        {
            int[] selectedWeapons = PlayerData.Instance.selectedWeapons;

            for (int i = 0; i < 5; i++)
            {
                GameObject buttonObj = Instantiate(buttonPrefab, transform);
                Button button = buttonObj.GetComponent<Button>();
                Image buttonImg = buttonObj.GetComponentsInChildren<Image>()[1];
                Image starImg = buttonObj.GetComponentsInChildren<Image>()[2];
                int weaponId = selectedWeapons[i];
                int index = i;

                int level = PlayerData.Instance.GetWeaponLevelFromId(weaponId);
                if (level > 0)
                {
                    buttonImg.sprite = WeaponManager.Instance.GetWeaponById(selectedWeapons[i]).weaponIconSprite;
                    button.onClick.AddListener(() => SwitchWeapon(index, weaponId));
                    starImg.sprite = GameAssets.i.stars[level - 1];
                }
                else
                {
                    buttonImg.sprite = weaponLockedSprite;
                    button.interactable = false;
                    starImg.gameObject.SetActive(false);
                }
                _weaponButtons.Add(button);
            }
            SwitchWeapon(0, selectedWeapons[0]);
        }
        
        private void Update() 
        {
            // Change Buttons' highlight state
            for (int i = 0; i < 5; i++)
            {
                if (i == _selectedIdx)
                {
                    _weaponButtons[i].Select();
                }
            }
        }

        private void SwitchWeapon(int index, int weaponId)
        {
            _selectedIdx = index;
            lp.SwitchWeapon(weaponId);
        }
    }
}
