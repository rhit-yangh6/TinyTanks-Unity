using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public class StarFlyCoordinator : ISubscriber, IRestart
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly UIStarsPanel _uiStarsPanel;
        private readonly AudioSource _pickupSound;
        private readonly WinPanel _winPanel;
        private readonly Star[] _stars;

        public StarFlyCoordinator(ICoroutineRunner coroutineRunner, UIStarsPanel uiStarsPanel, WinPanel winPanel,
            Star[] stars, AudioSource pickupSound)
        {
            _coroutineRunner = coroutineRunner;
            _uiStarsPanel = uiStarsPanel;
            _pickupSound = pickupSound;
            _winPanel = winPanel;
            _stars = stars;
        }

        void ISubscriber.Subscribe()
        {
            foreach (Star star in _stars)
            {
                star.Collected += OnStarCollected;
            }
        }

        void ISubscriber.Unsubscribe()
        {
            foreach (Star star in _stars)
            {
                star.Collected -= OnStarCollected;
            }
        }

        private void OnStarCollected(Star star)
        {
            _coroutineRunner.StartCoroutine(LaunchFlyOrder(star));
        }

        private IEnumerator LaunchFlyOrder(Star star)
        {
            _pickupSound.Play();
            int index = _uiStarsPanel.MoveNextStar();

            yield return star.Fly(_uiStarsPanel[index].GetWorldPosition(), 1.5f);
            yield return _uiStarsPanel[index].PlayAnimation();

            if (_uiStarsPanel.IsLastStar(index))
            {
                _uiStarsPanel.Hide();
                _winPanel.Show();
            }
        }

        public void Restart()
        {
            _coroutineRunner.StopAllCoroutines();
        }
    }
}