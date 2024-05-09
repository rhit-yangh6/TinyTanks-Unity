using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemosShared
{
    public class Volume : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Sprite _muteSprite;
        [SerializeField] private Sprite _unmuteSprite;
        [SerializeField] private Image _view;
        private bool _isMuted;

        private void Start()
        {
            AudioListener.volume = 1;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMuted = !_isMuted;
            _view.sprite = _isMuted ? _muteSprite : _unmuteSprite;
            AudioListener.volume = _isMuted ? 0 : 1;
        }
    }
}