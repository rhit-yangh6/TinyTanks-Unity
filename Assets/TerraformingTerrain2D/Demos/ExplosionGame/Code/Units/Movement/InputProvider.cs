using System;
using UnityEngine;

namespace ExplosionGame
{
    [Serializable]
    public class InputProvider
    {
        [SerializeField] private MovementButtons _movementButtons;

        public IInput GetInput()
        {
            #if UNITY_EDITOR
            return new DesktopInput();
            
            #elif UNITY_IOS || UNITY_ANDROID
            _movementButtons.Show();
            return new MobileInput(_movementButtons);

            #else
            return new DesktopInput();
            #endif
        }
    }
}