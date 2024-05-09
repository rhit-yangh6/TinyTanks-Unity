using UnityEngine;

namespace ExplosionGame
{
    public class MobileInput : IInput
    {
        private readonly MovementButtons _movementButtons;

        public MobileInput(MovementButtons movementButtons)
        {
            _movementButtons = movementButtons;
        }

        public Vector2 Direction => new(GetXDirection(), 0);

        private float GetXDirection()
        {
            float result = 0;

            if (_movementButtons.Left.IsTouching)
            {
                result = -1;
            }
            else if (_movementButtons.Right.IsTouching)
            {
                result = 1;
            }

            return result;
        }
    }
}