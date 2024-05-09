using DemosShared;

namespace ExplosionGame
{
    public class UnitStateMachineFactory
    {
        private readonly PhysicsOverlapCheck _overlapCheck;
        private readonly UnitViewComponents _components;
        private readonly MovementState _movementState;
        private readonly DieState _dieState;
        private readonly float _speed;

        public UnitStateMachineFactory(UnitViewComponents components, MovementState movementState,
            PhysicsOverlapCheck overlapCheck, DieState dieState, float speed = 1)
        {
            _overlapCheck = overlapCheck;
            _movementState = movementState;
            _components = components;
            _dieState = dieState;
            _speed = speed;
        }

        public StateMachine Create()
        {
            TransitionsConditions transitionsConditions = new(_movementState, _overlapCheck);
            StatesPresenter states = new()
            {
                Idle = new IdleState(CreateState("Idle"), _components.Rigidbody),
                Fall = new FallState(CreateState("Fall")),
                Run = new RunState(CreateState("Walk"), _components.Rigidbody, _movementState, _speed),
                Die = _dieState,
            };

            TransitionsPresenterFactory transitionsPresenterFactory = new(states, transitionsConditions);
            TransitionsPresenter transitionsPresenter = transitionsPresenterFactory.Create();

            return new StateMachine(transitionsPresenter, states.Idle);
        }

        private AnimationState CreateState(string animationName)
        {
            return new AnimationState(_components.Animator, animationName);
        }
    }
}