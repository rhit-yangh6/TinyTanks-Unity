using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.GameEngine.FieldPickups
{
    public class WeaponPickup : FieldPickup
    {
        // Set in Inspector
        [SerializeField] private int weaponId;
        
        private SpriteRenderer _sr;
        private void Start()
        {
            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            _sr = GetComponentInChildren<SpriteRenderer>();
            _sr.sprite = weapon.weaponIconSprite;
        }

        protected override void Trigger()
        {
            WeaponManager.UnlockWeapon(weaponId);
        }
    }
}