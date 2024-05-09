using System;
using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    [Serializable]
    public class MovementButtons
    {
        [SerializeField] private InteractiveUI _leftButton;
        [SerializeField] private InteractiveUI _rightButton;

        public InteractiveUI Left => _leftButton;
        public InteractiveUI Right => _rightButton;

        public void Show()
        {
            Left.gameObject.SetActive(true);
            Right.gameObject.SetActive(true);
        }
    }
}