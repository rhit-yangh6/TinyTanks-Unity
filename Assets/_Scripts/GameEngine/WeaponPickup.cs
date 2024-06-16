using System;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.GameEngine
{
    public class WeaponPickup : MonoBehaviour
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            WeaponManager.UnlockWeapon(weaponId);
            Destroy(gameObject);
        }
    }
}