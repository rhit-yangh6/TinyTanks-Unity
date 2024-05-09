using UnityEngine;

namespace ExplosionGame
{
    public class MovementState
    {
        private readonly IInput _input;

        public MovementState(IInput input)
        {
            _input = input;
        }

        public Vector2 Direction => _input.Direction;
        public bool IsStaying => _input.Direction == Vector2.zero;
        public bool IsMovingLeft => _input.Direction.x < 0;
    }
}