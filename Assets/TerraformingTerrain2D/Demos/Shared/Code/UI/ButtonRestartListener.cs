using UnityEngine.UI;

namespace DemosShared
{
    public class ButtonRestartListener : ISubscriber
    {
        private readonly Button _restartButton;
        private readonly IRestart _restart;

        public ButtonRestartListener(Button restartButton, IRestart restart)
        {
            _restartButton = restartButton;
            _restart = restart;
        }

        void ISubscriber.Subscribe()
        {
            _restartButton.onClick.AddListener(OnRestart);
        }

        void ISubscriber.Unsubscribe()
        {
            _restartButton.onClick.RemoveListener(OnRestart);
        }

        private void OnRestart()
        {
            _restart.Restart();
        }
    }
}