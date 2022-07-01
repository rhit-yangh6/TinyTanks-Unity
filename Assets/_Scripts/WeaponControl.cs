using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class WeaponControl : MonoBehaviour
    {

        public LaunchProjectile lp;
        public GameObject buttonPrefab;
        public Sprite weaponLockedSprite;

        
        private void Start()
        {
            // TODO: Load Selected Weapons
            int[] selectedWeapons = PlayerData.Instance.selectedWeapons;

            for (int i = 0; i < 5; i++)
            {
                GameObject buttonObj = Instantiate(buttonPrefab, transform);
                Image buttonImg = buttonObj.GetComponent<Image>();
                Button button = buttonObj.GetComponent<Button>();
                Image starImg = buttonObj.GetComponentsInChildren<Image>()[1];
                int weaponId = selectedWeapons[i];

                int level = PlayerData.Instance.GetWeaponLevelFromId(weaponId);
                if (level > 0)
                {
                    buttonImg.sprite = WeaponManager.Instance.GetWeaponById(selectedWeapons[i]).weaponIconSprite;
                    button.onClick.AddListener(() => SwitchWeapon(weaponId));
                    starImg.sprite = GameAssets.i.stars[level - 1];
                }
                else
                {
                    buttonImg.sprite = weaponLockedSprite;
                    button.interactable = false;
                    starImg.gameObject.SetActive(false);
                    // starImg.sprite = null;
                }
            }
            SwitchWeapon(selectedWeapons[0]);
        }

        private void SwitchWeapon(int weaponId)
        {
            lp.selectedWeaponId = weaponId;
            lp.RefreshPrefab();
        }
    }
}
