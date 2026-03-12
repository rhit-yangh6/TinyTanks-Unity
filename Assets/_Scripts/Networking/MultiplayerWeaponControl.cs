using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.UI.WeaponExternalDisplay;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Networking
{
    /// <summary>
    /// Weapon selection panel for the local multiplayer player.
    /// Only visible during the local player's turn.
    /// </summary>
    public class MultiplayerWeaponControl : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject weaponExtraDisplayPanel;

        private MultiplayerLaunchProjectile _launcher;
        private readonly List<Button> _weaponButtons = new();
        private List<GameEngine.WeaponExtraData.WeaponExtraData> _weaponExtraData = new();
        private (int weaponId, int level)[] _weapons;
        private int _selectedIdx;

        public void Initialize(MultiplayerLaunchProjectile launcher, (int weaponId, int level)[] weapons)
        {
            _launcher = launcher;
            _weapons = weapons;

            CreateButtons();

            if (_weapons.Length > 0)
            {
                SwitchWeapon(0, _weapons[0].weaponId, _weapons[0].level, _weaponExtraData[0]);
            }
        }

        private void CreateButtons()
        {
            // Clear existing
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            _weaponButtons.Clear();
            _weaponExtraData.Clear();

            for (int i = 0; i < _weapons.Length && i < 5; i++)
            {
                var (weaponId, level) = _weapons[i];
                var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
                if (weapon == null) continue;

                GameObject buttonObj = Instantiate(buttonPrefab, transform);
                Button button = buttonObj.GetComponent<Button>();
                Image buttonImg = buttonObj.GetComponentsInChildren<Image>()[0];
                var animator = buttonImg.GetComponent<Animator>();
                Image starImg = buttonObj.GetComponentsInChildren<Image>()[2];

                _weaponExtraData.Add(weapon.WeaponExtraData);

                buttonImg.sprite = weapon.weaponIconSprite;
                int index = i;
                var wed = weapon.WeaponExtraData;
                button.onClick.AddListener(() => SwitchWeapon(index, weaponId, level, wed));
                starImg.sprite = GameAssets.i.stars[level - 1];

                if (weaponId >= 1000)
                {
                    animator.runtimeAnimatorController =
                        Resources.Load<RuntimeAnimatorController>("AnimatorControllers/" + weapon.dataPath);
                    animator.enabled = true;
                }
                else
                {
                    animator.enabled = false;
                }

                _weaponButtons.Add(button);
            }
        }

        private void Update()
        {
            for (int i = 0; i < _weaponButtons.Count; i++)
            {
                if (i == _selectedIdx)
                    _weaponButtons[i].Select();
            }
        }

        private void SwitchWeapon(int index, int weaponId, int level,
            GameEngine.WeaponExtraData.WeaponExtraData wed)
        {
            // Clear extra display
            foreach (Transform child in weaponExtraDisplayPanel.transform)
                Destroy(child.gameObject);

            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            if (weapon.hasExternalDisplay)
            {
                var display = Instantiate(weapon.weaponExternalDisplay, weaponExtraDisplayPanel.transform)
                    .GetComponent<WeaponExternalDisplay>();
                display.UpdateDisplay(wed);
            }

            _selectedIdx = index;
            _launcher.SwitchWeapon(weaponId, level, wed);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
