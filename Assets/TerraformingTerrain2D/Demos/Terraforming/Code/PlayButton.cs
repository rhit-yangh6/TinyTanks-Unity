using System.Collections.Generic;
using System.Linq;
using DemosShared;
using UnityEngine;
using UnityEngine.UI;

namespace PaintingGame
{
    public class PlayButton : MonoBehaviour, IRestart
    {
        private IEnumerable<IPlay> _starts;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _restartButton;
        
        public void Compose()
        {
            _starts = Utils.FindObjects<MonoBehaviour>().OfType<IPlay>();
        }

        public void StartComponents()
        {
            ToggleButton(_restartButton);

            foreach (IPlay start in _starts)
            {
                start.Play();
            }
        }

        void IRestart.Restart()
        {
            ToggleButton(_playButton);
        }

        private void ToggleButton(Button button)
        {
            _playButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
            
            button.gameObject.SetActive(true);
        }
    }
}