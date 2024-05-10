using System.Linq;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI.Shop
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private GameObject shopWeaponButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private GameObject weaponScrollListContent;
        [SerializeField] private ShopDetailPanel sdp;

        private readonly int[] achievementWeapons =
        {
            // Empty for now
        };
        private readonly int[] promoCodeWeapons =
        {
            // Empty for now
        };
        private readonly int[] bossWeapons =
        {
            25, // AirStrike
        };
        private readonly int[] enhancedWeapons =
        {
            1000, // AirStrike (Enhanced)
        };
        private readonly int[] unobtainableWeapons =
        {
            -1, // Cannon
        };

        private void Start()
        {
            backButton.onClick.AddListener(SaveSystem.SavePlayer);
        }

        private void OnEnable ()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceMenuDetail,
                Constants.RichPresenceShopState);
            PopulateWeaponIcons();
            coinText.text = PlayerData.Instance.coins.ToString();
        }

        private void OnDisable()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceMenuDetail,
                Constants.RichPresenceMenuState);
        }

        private void PopulateWeaponIcons()
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
                var finalHiddenList = achievementWeapons
                    .Concat(promoCodeWeapons)
                    .Concat(bossWeapons)
                    .Concat(enhancedWeapons)
                    .Concat(unobtainableWeapons)
                    .ToArray();

                // Won't displayed in shop
                if (PlayerData.Instance.GetWeaponLevelFromId(w.id) > 0 ||
                    finalHiddenList.Contains(w.id)) continue;
                
                var buttonObj = Instantiate(shopWeaponButton, weaponScrollListContent.transform);
                var s = buttonObj.GetComponent<Image>();
                var button = buttonObj.GetComponent<Button>();
                var weaponId = w.id;

                s.sprite = w.weaponIconSprite; 
                button.onClick.AddListener(() => sdp.SetDetails(weaponId));
            }
        }
    }
}
