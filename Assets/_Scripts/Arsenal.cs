using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class Arsenal : MonoBehaviour
    {

        [SerializeField] private GameObject arsenalWeaponButton;
        public GameObject weaponScrollListContent;
        public TextMeshProUGUI coinText;
        public WeaponDetailPanel wdp;
        public Sprite weaponLockedSprite;
        [SerializeField] private Button backButton;

        private void Start()
        {
            backButton.onClick.AddListener(SaveSystem.SavePlayer);
        }
        
        void OnEnable ()
        {
            PopulateWeaponIcons();
            wdp.SwitchDetailView();
            coinText.text = PlayerData.Instance.coins.ToString();
        }

        private void PopulateWeaponIcons()
        {
            foreach (Transform child in weaponScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            
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

                    s.GetComponent<DragDropGrid>().weaponId = weaponId;
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
