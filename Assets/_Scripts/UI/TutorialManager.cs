using System;
using _Scripts.Managers;
using Unity.VisualScripting;
using UnityEngine;
using EventBus = _Scripts.Managers.EventBus;

namespace _Scripts.UI
{
    public class TutorialManager: MonoBehaviour
    {
        public GameObject[] popUps;
        
        /*
         * 0 -> A, D Movement
         * 1 -> Drag to aim
         * 2 -> Release to shoot
         * 3 -> Select Weapon
         * 4 -> Special Effect
         */
        public int popUpIndex;
        private bool _dragged;
        private bool _released;

        private void Start()
        {
            EventBus.AddListener(EventTypes.StartedDragging, HandleStartDragging);
            EventBus.AddListener(EventTypes.StoppedDragging, HandleStopDragging);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener(EventTypes.StartedDragging, HandleStartDragging);
            EventBus.RemoveListener(EventTypes.StoppedDragging, HandleStopDragging);
        }

        private void Update()
        {
            for (var i = 0; i < popUps.Length; i++)
            {
                popUps[i].SetActive(i == popUpIndex);
            }
            
            if (popUpIndex == 0)
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    popUpIndex++;
                }
            }
        }

        private void HandleStartDragging()
        {
            if (_dragged || popUpIndex != 1) return;

            _dragged = true;
            popUpIndex++;
        }

        private void HandleStopDragging()
        {
            if (_released || popUpIndex != 2) return;
            
            _released = true;
            popUpIndex++;
        }
        
        public void HandleWeaponSelected()
        {
            popUpIndex++;
        }
    }
}