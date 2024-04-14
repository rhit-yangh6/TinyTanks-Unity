using _Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class NewWeaponMsgBehavior : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponName, weaponDesc;

        [HideInInspector] public int weaponId;
        
        private void OnEnable()
        {
            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            
            weaponImage.sprite = weapon.weaponIconSprite;
            weaponName.text = weapon.weaponName;
            weaponDesc.text = weapon.weaponDescription;
        }
    }
}
