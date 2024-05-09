using UnityEngine;

namespace ExplosionGame
{
    public class WeaponChargeView : MonoBehaviour
    {
        [SerializeField] private WeaponChargeViewGeneration _viewGeneration;

        [ContextMenu("Regenerate")]
        public void RegenerateCircles()
        {
            _viewGeneration.Regenerate(transform);
        }

        public void SetCharge(float charge)
        {
            float singleLerpValue = (float)1 / _viewGeneration.Circles.Length;
            int circlesToShow = (int)(charge / singleLerpValue);

            for (int i = 0; i < circlesToShow; ++i)
            {
                _viewGeneration.Circles[i].gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            foreach (Transform circle in _viewGeneration.Circles)
            {
                circle.gameObject.SetActive(false);
            }
        }
    }
}