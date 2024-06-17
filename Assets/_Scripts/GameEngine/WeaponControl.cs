using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.UI;
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
                Image buttonImg = buttonObj.GetComponentsInChildren<Image>()[0];
                var animator = buttonImg.GetComponent<Animator>();
                Image starImg = buttonObj.GetComponentsInChildren<Image>()[2];
                
                var selectionDatum = selectedWeapons[i];
                
                if (selectedWeapons[i] != null)
                {
                    var index = i;
                    var weapon = WeaponManager.Instance.GetWeaponById(selectionDatum.weaponId);
                    buttonImg.sprite = weapon.weaponIconSprite;
                    button.onClick.AddListener(() => SwitchWeapon(index, selectionDatum));
                    starImg.sprite = GameAssets.i.stars[selectionDatum.level - 1];
                    
                    // Enhanced?
                    if (selectedWeapons[i].weaponId >= 1000)
                    {
                        animator.runtimeAnimatorController =
                            Resources.Load<RuntimeAnimatorController>("AnimatorControllers/" + weapon.dataPath);
                        animator.enabled = true;
                    }
                    else
                    {
                        animator.enabled = false;
                    }
                    
                    // Tooltip
                    buttonObj.GetComponent<SelectionWeaponButton>().weaponId = selectionDatum.weaponId;
                }
                else
                {
                    buttonImg.sprite = GameAssets.i.weaponEmptySprite;
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
