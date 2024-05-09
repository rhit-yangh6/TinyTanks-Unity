
using DemosShared;

namespace ExplosionGame
{
    public class DieRestart : ISubscriber
    {
        private readonly DieState _dieState;
        private readonly IRestart _restart;

        public DieRestart(DieState dieState, IRestart restart)
        {
            _dieState = dieState;
            _restart = restart;
        }

        void ISubscriber.Subscribe()
        {
            _dieState.Entered += OnRestart;
        }

        void ISubscriber.Unsubscribe()
        {
            _dieState.Entered -= OnRestart;
        }

        private void OnRestart()
        {
            _restart.Restart();
        }
    }
}