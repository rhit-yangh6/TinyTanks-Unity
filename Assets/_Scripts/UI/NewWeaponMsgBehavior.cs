using _Scripts.Managers;
using Discord;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class NewWeaponMsgBehavior : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponName, weaponDesc;
        
        public void Display(int weaponId)
        {
            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            
            weaponImage.sprite = weapon.weaponIconSprite;
            weaponName.text = weapon.weaponName;
            weaponDesc.text = weapon.weaponDescription;
            
            // Enhanced?
            var animator = weaponImage.GetComponent<Animator>();
            if (weaponId >= 1000)
            {
                animator.runtimeAnimatorController =
                    Resources.Load<RuntimeAnimatorController>("AnimatorControllers/" + weapon.dataPath + "_enhanced");
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
            Destroy(gameObject);
        }
    }
}
