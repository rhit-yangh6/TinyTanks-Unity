using _Scripts.Managers;
using _Scripts.Utils;
using Discord;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class NewWeaponMsgBehavior : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private LocalizeStringEvent weaponNameEvent, weaponDescEvent;
        
        public void Display(int weaponId)
        {
            EventBus.Broadcast(EventTypes.PauseGame);
            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            
            weaponImage.sprite = weapon.weaponIconSprite;
            
            weaponNameEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponName);
            weaponDescEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableWeaponText, weapon.weaponDescription);
            
            // Enhanced?
            var animator = weaponImage.GetComponent<Animator>();
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
            
            gameObject.SetActive(true);
        }

        public void OnClose()
        {
            EventBus.Broadcast(EventTypes.ResumeGame);
            Destroy(gameObject);
        }
    }
}
