using System;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.UI.Arsenal
{
    public class Arsenal : MonoBehaviour, IDropHandler
    {
        [SerializeField] private GameObject arsenalWeaponButton;
        [SerializeField] private Button backButton;
        [SerializeField] public GameObject weaponScrollListContent;
        [SerializeField] public TextMeshProUGUI coinText;
        [SerializeField] public ArsenalWeaponDetailPanel wdp;
        [SerializeField] private Image sortButtonImage;
        [SerializeField] private Sprite[] sortIcons;

        /*
         * 0 - sort by id
         * 1 - sort by name
         * 2 - sort by level
         */
        private int sortMode;
        
        private void Start()
        {
            // backButton.onClick.AddListener(SaveSystem.SavePlayer);
            EventBus.AddListener<int>(EventTypes.WeaponUnlocked, PopulateWeaponIcons);
        }

        public void SwitchSortMode()
        {
            sortMode = (sortMode + 1) % sortIcons.Length;
            sortButtonImage.sprite = sortIcons[sortMode];
            PopulateWeaponIcons(0);
        }

        private void OnEnable()
        {
            PopulateWeaponIcons(0);
            wdp.SwitchDetailView();
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
            Array.Sort(weapons,
                delegate(Weapon w1, Weapon w2) {  
                    var hasW1 = (PlayerData.Instance.GetWeaponLevelFromId(w1.id) > 0) ? 1 : 0;
                    var hasW2 = (PlayerData.Instance.GetWeaponLevelFromId(w2.id) > 0) ? 1 : 0;
                    if (hasW1 != hasW2)
                    {
                        return hasW2.CompareTo(hasW1);
                    }

                    switch (sortMode)
                    {
                        case 0:
                            return w1.id.CompareTo(w2.id);
                        case 1:
                            return string.Compare(w1.dataPath, w2.dataPath, StringComparison.Ordinal);
                        case 2:
                            var w1Level = PlayerData.Instance.GetWeaponLevelFromId(w1.id);
                            var w2Level = PlayerData.Instance.GetWeaponLevelFromId(w2.id);
                            if (w1Level >= 5 && w2Level >= 5)
                            {
                                return w1.id.CompareTo(w2.id);
                            }

                            return w2Level.CompareTo(w1Level);
                    }

                    return 0;
                });

            weapons = Array.FindAll(weapons, w =>
            {
                // Keep the weapon if you own this weapon
                if (PlayerData.Instance.GetWeaponLevelFromId(w.id) > 0) return true;
                
                // Do not show hidden ones
                return !w.hideInArsenal;
            });

            foreach (var w in weapons)
            {
                var buttonObj = Instantiate(arsenalWeaponButton, weaponScrollListContent.transform);
                var s = buttonObj.GetComponent<Image>();
                var animator = buttonObj.GetComponent<Animator>();
                var button = buttonObj.GetComponent<Button>();
                var newLabelCanvasGroup = buttonObj.GetComponentInChildren<CanvasGroup>();
                var weaponId = w.id;

                if (PlayerData.Instance.GetWeaponLevelFromId(weaponId) > 0)
                {
                    s.sprite = w.weaponIconSprite;

                    button.onClick.AddListener(() =>
                    {
                        newLabelCanvasGroup.alpha = 0;
                        PlayerData.Instance.CheckWeapon(weaponId);
                        wdp.SetDetails(weaponId);
                    });

                    s.GetComponent<DragDropGrid>().weaponId = weaponId;

                    // Enhanced?
                    if (weaponId >= 1000)
                    {
                        animator.runtimeAnimatorController =
                            Resources.Load<RuntimeAnimatorController>("AnimatorControllers/" + w.dataPath);
                        animator.enabled = true;
                    }
                    else animator.enabled = false;

                    newLabelCanvasGroup.alpha = PlayerData.Instance.IsWeaponChecked(weaponId) ? 0 : 1;
                }
                else
                {
                    s.sprite = GameAssets.i.weaponLockedSprite;
                    button.interactable = false;
                }
                
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            var ddi = eventData.pointerDrag.GetComponent<DragDropIcon>();

            if (!ddi) return;
            
            var incomingWeaponId = ddi.weaponId;
            if (incomingWeaponId == 0) return;

            var isClearSuccessful = PlayerData.Instance.ClearWeaponSelection(incomingWeaponId);

            if (isClearSuccessful) ddi.SetSprite(0);
        }
    }
}
