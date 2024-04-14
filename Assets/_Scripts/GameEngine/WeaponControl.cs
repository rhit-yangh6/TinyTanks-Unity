using System.Collections.Generic;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.GameEngine
{
    public class WeaponControl : MonoBehaviour
    {

        public GameObject buttonPrefab;
        private readonly List<Button> _weaponButtons = new ();
        private int _selectedIdx;
        private GameObject _player;
        private LaunchProjectile _lp;

        private void Start()
        {
            SelectionDatum[] selectedWeapons = PlayerData.Instance.selectedWeapons;
            
            _player = GameObject.FindGameObjectWithTag("Player");
            _lp = _player.GetComponent<LaunchProjectile>();

            for (int i = 0; i < 5; i++)
            {
                GameObject buttonObj = Instantiate(buttonPrefab, transform);
                Button button = buttonObj.GetComponent<Button>();
                Image buttonImg = buttonObj.GetComponentsInChildren<Image>()[1];
                Image starImg = buttonObj.GetComponentsInChildren<Image>()[2];
                
                var selectionDatum = selectedWeapons[i];
                
                if (selectedWeapons[i] != null)
                {
                    var index = i;
                    
                    buttonImg.sprite = WeaponManager.Instance.GetWeaponById(selectionDatum.weaponId).weaponIconSprite;
                    button.onClick.AddListener(() => SwitchWeapon(index, selectionDatum));
                    starImg.sprite = GameAssets.i.stars[selectionDatum.level - 1];
                }
                else
                {
                    buttonImg.sprite = GameAssets.i.weaponLockedSprite;
                    button.interactable = false;
                    starImg.gameObject.SetActive(false);
                }
                _weaponButtons.Add(button);
            }
            SwitchWeapon(_selectedIdx, selectedWeapons[0]);
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

        private void SwitchWeapon(int index, SelectionDatum sd)
        {
            _selectedIdx = index;
            _lp.SwitchWeapon(sd);
        }
    }
}
