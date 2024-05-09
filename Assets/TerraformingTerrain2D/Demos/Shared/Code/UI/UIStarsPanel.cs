using UnityEngine;

namespace DemosShared
{
    public class UIStarsPanel : UIPanel
    {
        [SerializeField] private UIStar[] _uiStars;
        private int _currentIndex;

        public int MoveNextStar()
        {
            return _currentIndex++;
        }

        public UIStar this[int i] => _uiStars[i];

        public bool IsLastStar(int index) => index == _uiStars.Length - 1;

        protected override void OnRestart()
        {
            _currentIndex = 0;
        }
    }
}