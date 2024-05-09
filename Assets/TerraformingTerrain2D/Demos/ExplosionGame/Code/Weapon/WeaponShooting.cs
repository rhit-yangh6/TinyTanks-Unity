using System.Collections.Generic;
using DemosShared;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ExplosionGame
{
    public class WeaponShooting : IUpdate
    {
        private readonly WeaponHolder _weaponHolder;

        public WeaponShooting(WeaponHolder weaponHolder)
        {
            _weaponHolder = weaponHolder;
        }

        void IUpdate.Update()
        {
            if (Input.GetMouseButton(0) && IsPointerOverUIObject() == false)
            {
                _weaponHolder.Shoot();
            }
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}