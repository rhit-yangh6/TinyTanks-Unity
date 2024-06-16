using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.GameEngine
{
    public class TutorialWeaponControl : MonoBehaviour
    {
        public TutorialManager tutorialManager;
        public GameObject buttonPrefab;
        private readonly List<Button> _weaponButtons = new ();
        private int _selectedIdx;
        private GameObject _player;
        private LaunchProjectile _lp;

        private void Start()
        {
            var selectedWeapons = new SelectionDatum[]
            {
                new (1, 1), // Boulder
                new (3, 1), // Rocket
                null,
                null,
                null
            };
            
            _player = GameObject.FindGameObjectWithTag("Player");
            _lp = _player.GetComponentsInChildren<LaunchProjectile>()[0];

            for (int i = 0; i < 5; i++)
            {
                GameObject buttonObj = Instantiate(buttonPrefab, transform);
                Button button = buttonObj.GetComponent<Button>();
                Image buttonImg = buttonObj.GetComponentsInChildren<Image>()[0];
                Image starImg = buttonObj.GetComponentsInChildren<Image>()[2];
                
                var selectionDatum = selectedWeapons[i];
                
                if (selectedWeapons[i] != null)
                {
                    var index = i;
                    
                    buttonImg.sprite = WeaponManager.Instance.GetWeaponById(selectionDatum.weaponId).weaponIconSprite;
                    button.onClick.AddListener(() => SwitchWeapon(index, selectionDatum));
                    starImg.sprite = GameAssets.i.stars[selectionDatum.level - 1];
                    
                    // Tooltip
                    buttonObj.GetComponent<SelectionWeaponButton>().weaponId = selectionDatum.weaponId;
                }
                else
                {
                    buttonImg.sprite = GameAssets.i.weaponLockedSprite;
                    button.interactable = false;
                    starImg.gameObject.SetActive(false);
                }
                _weaponButtons.Add(button);
            }
            SwitchWeapon(_selectedIdx, selectedWeapons[0], true);
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

        private void SwitchWeapon(int index, SelectionDatum sd, bool bypass = false)
        {
            if (bypass || (tutorialManager.popUpIndex == 3 && index == 1))
            {
                _selectedIdx = index;
                _lp.SwitchWeapon(sd);
                if (!bypass)
                {
                    tutorialManager.HandleWeaponSelected();
                }
            }
        }
    }
}
