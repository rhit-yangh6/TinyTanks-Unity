using System.Collections.Generic;

namespace DemosShared
{
    public class RestartComposite : IRestart
    {
        private readonly IEnumerable<IRestart> _restartable;

        public RestartComposite(IEnumerable<IRestart> restartable)
        {
            _restartable = restartable;
        }

        public void Restart()
        {
            foreach (IRestart restartable in _restartable)
            {
                restartable.Restart();
            }
        }
    }
}