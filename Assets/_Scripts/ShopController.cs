using System;
using _Scripts.Arsenal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private GameObject shopWeaponButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private GameObject weaponScrollListContent;
        [SerializeField] private ShopDetailPanel sdp;

        private void Start()
        {
            backButton.onClick.AddListener(SaveSystem.SavePlayer);
        }

        private void OnEnable ()
        {
            PopulateWeaponIcons();
            coinText.text = PlayerData.Instance.coins.ToString();
        }

        private void PopulateWeaponIcons()
        {
            foreach (Transform child in weaponScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            
            Weapon[] weapons = WeaponManager.Instance.GetAllWeapons();
            /*
            Array.Sort(weapons,
                delegate(Weapon w1, Weapon w2) {  
                    var hasW1 = (PlayerData.Instance.GetWeaponLevelFromId(w1.id) > 0) ? 1 : 0;
                    var hasW2 = (PlayerData.Instance.GetWeaponLevelFromId(w2.id) > 0) ? 1 : 0;
                    if (hasW1 == hasW2)
                    {
                        return w1.id.CompareTo(w2.id);
                    }
                    else
                    {
                        return hasW2.CompareTo(hasW1);
                    } 
                });
                */

            foreach (Weapon w in weapons)
            {

                // Won't displayed in shop
                if (PlayerData.Instance.GetWeaponLevelFromId(w.id) > 0 || w.shopPrice == 0)
                {
                    continue;
                }
                
                var buttonObj = Instantiate(shopWeaponButton, weaponScrollListContent.transform);
                Image s = buttonObj.GetComponent<Image>();
                Button button = buttonObj.GetComponent<Button>();
                int weaponId = w.id;

                s.sprite = w.weaponIconSprite; 
                button.onClick.AddListener(() => sdp.SetDetails(weaponId));
            }
        }
    }
}
