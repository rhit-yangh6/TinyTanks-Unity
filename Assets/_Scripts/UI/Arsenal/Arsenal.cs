using System;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI.Arsenal
{
    public class Arsenal : MonoBehaviour
    {

        [SerializeField] private GameObject arsenalWeaponButton;
        [SerializeField] private Button backButton;

        public GameObject weaponScrollListContent;
        public TextMeshProUGUI coinText;
        public ArsenalWeaponDetailPanel wdp;

        private void Start()
        {
            backButton.onClick.AddListener(SaveSystem.SavePlayer);
        }

        private void OnEnable ()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceMenuDetail,
                Constants.RichPresenceArsenalState);
            PopulateWeaponIcons();
            wdp.SwitchDetailView();
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
            
            Weapon[] weapons = WeaponManager.Instance.GetAllWeapons();
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

            weapons = Array.FindAll(weapons, w =>
            {
                // Keep the weapon if you own this weapon
                if (PlayerData.Instance.GetWeaponLevelFromId(w.id) > 0)
                {
                    return true;
                }
            
                // Do not show debugger
                return w.id != -1;
            });

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

                    s.GetComponent<DragDropGrid>().weaponId = weaponId;
                }
                else
                {
                    s.sprite = GameAssets.i.weaponLockedSprite;
                    button.interactable = false;
                }
                
            }
        }

    }
}