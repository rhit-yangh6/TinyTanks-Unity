using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    public class TransitionsConditions
    {
        private readonly MovementState _movementState;
        private readonly PhysicsOverlapCheck _overlapCheck;

        public TransitionsConditions(MovementState movementState, PhysicsOverlapCheck overlapCheck)
        {
            _movementState = movementState;
            _overlapCheck = overlapCheck;
        }

        public bool CheckIfStaying() => _movementState.IsStaying;

        public bool CheckIfRun() => CheckIfStaying() == false;

        public bool CheckIfGrounded() => _overlapCheck.CheckCollision<Ground>();

        public bool CheckIfFall() => CheckIfGrounded() == false;

        public bool CheckIfDroppedIntoWater() => _overlapCheck.HasCollision(LayerMask.GetMask("Water"));
    }
}