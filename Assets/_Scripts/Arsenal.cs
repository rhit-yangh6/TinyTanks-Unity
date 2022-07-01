using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class Arsenal : MonoBehaviour
    {

        public GameObject arsenalWeaponButton;
        public GameObject weaponScrollListContent;
        public TextMeshProUGUI coinText;
        public WeaponDetailPanel wdp;
        public Sprite weaponLockedSprite;
    
        // Start is called before the first frame update
        void Start()
        {
            PopulateWeaponIcons();

            coinText.text = PlayerData.Instance.coins.ToString();
        }

        private void PopulateWeaponIcons()
        {
            Weapon[] weapons = WeaponManager.Instance.GetAllWeapons();

            foreach (Weapon w in weapons)
            {
                GameObject buttonObj = Instantiate(arsenalWeaponButton, weaponScrollListContent.transform);
                Image s = buttonObj.GetComponent<Image>();
                Button button = buttonObj.GetComponent<Button>();
                int weaponId = w.id;

                if (PlayerData.Instance.GetWeaponLevelFromId(weaponId) > 0)
                {
                    s.sprite = w.weaponIconSprite;

                    button.onClick.AddListener(() => wdp.SetDetails(weaponId));
                }
                else
                {
                    s.sprite = weaponLockedSprite;
                    button.interactable = false;
                }
                
            }
        }
    
    }
}
