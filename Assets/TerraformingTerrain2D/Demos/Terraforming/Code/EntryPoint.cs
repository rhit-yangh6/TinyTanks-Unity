using TerraformingTerrain2d;
using DemosShared;
using UnityEngine;

namespace PaintingGame
{
    [DefaultExecutionOrder(-10)]
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private TerraformingModeButton _carveButton;
        [SerializeField] private TerraformingModeButton _fillButton;
        [SerializeField] private ServicesRunner _servicesRunner;
        [SerializeField] private PlayButton _playButton;
        [SerializeField] private Star[] _stars;
        [SerializeField] private Brush _brush;

        private void Awake()
        {
            _playButton.Compose();

            foreach (Star star in _stars)
            {
                star.Compose();
            }
            
            _servicesRunner.Compose(_stars);
            TryEnableTerraformButtons();
        }

        private void TryEnableTerraformButtons()
        {
            #if UNITY_ANDROID || UNITY_IOS
            if (Application.isEditor == false)
            {
                _carveButton.gameObject.SetActive(true);
                _fillButton.gameObject.SetActive(true);
                _carveButton.Compose(_brush, TerraformingMode.Carve);
                _fillButton.Compose(_brush, TerraformingMode.Fill);    
            }
            #endif
        }
    }
}
