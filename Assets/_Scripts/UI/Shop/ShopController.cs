using System.Linq;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI.Shop
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private GameObject shopWeaponButton, blurPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private GameObject weaponScrollListContent;
        [SerializeField] private ShopDetailPanel sdp;
        [SerializeField] private ModalWindowManager modalWindowManager;
        [SerializeField] private BlurManager blurManager;

        private void Start()
        {
            // backButton.onClick.AddListener(SaveSystem.SavePlayer);
            EventBus.AddListener<int>(EventTypes.WeaponUnlocked, PopulateWeaponIcons);
        }

        private void OnEnable ()
        {
            PopulateWeaponIcons(0);
            // coinText.text = PlayerData.Instance.coins.ToString();
        }
        
        private void OnDestroy()
        {
            EventBus.RemoveListener<int>(EventTypes.WeaponUnlocked, PopulateWeaponIcons);
        }

        private void PopulateWeaponIcons(int unusedWeaponId)
        {
            foreach (Transform child in weaponScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            
            var weapons = WeaponManager.Instance.GetAllWeapons();
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

            foreach (var w in weapons)
            {
                // Won't displayed in shop
                if (PlayerData.Instance.GetWeaponLevelFromId(w.id) > 0 ||
                    w.hideInShop) continue;
                
                var buttonObj = Instantiate(shopWeaponButton, weaponScrollListContent.transform);
                var s = buttonObj.GetComponentInChildren<Image>();
                var button = buttonObj.GetComponentInChildren<Button>();
                var buttonCoinText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                var weaponId = w.id;

                buttonCoinText.text = w.shopPrice.ToString();
                s.sprite = w.weaponIconSprite; 
                button.onClick.AddListener(() =>
                {
                    modalWindowManager.ModalWindowIn();
                    blurManager.BlurInAnim();
                    sdp.SetDetails(weaponId);
                    // blurPanel.SetActive(true);
                    // sdp.gameObject.SetActive(true);
                    // sdp.SetDetails(weaponId);
                });
            }
        }
    }
}
