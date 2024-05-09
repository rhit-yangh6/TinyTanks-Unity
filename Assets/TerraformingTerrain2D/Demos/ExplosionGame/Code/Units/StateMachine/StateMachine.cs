using DemosShared;

namespace ExplosionGame
{
    public class StateMachine : IFixedUpdate, IUpdate, IRestart
    {
        private readonly TransitionsPresenter _transitionsPresenter;
        private readonly State _initialState;
        private State _state;

        public StateMachine(TransitionsPresenter transitionsPresenter, State initialState)
        {
            _transitionsPresenter = transitionsPresenter;
            _initialState = initialState;
            _state = initialState;
        }

        void IFixedUpdate.FixedUpdate()
        {
            _state.FixedUpdate();
        }

        void IUpdate.Update()
        {
            _state.Update();
            TryTransit();
        }

        private void TryTransit()
        {
            State newState = _transitionsPresenter.Transit(_state);

            if (newState != _state)
            {
                SwitchState(newState);
            }
        }

        private void SwitchState(State newState)
        {
            _state.Exit();
            _state = newState;
            _state.Enter();
        }

        public void Restart()
        {
            SwitchState(_initialState);
        }
    }
}