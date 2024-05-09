using TerraformingTerrain2d;
using DemosShared;
using UnityEngine;
using UnityEngine.UI;

namespace PaintingGame
{
    public class TerraformingModeButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        private TerraformingMode _terraformingMode;
        private Brush _brush;

        public void Compose(Brush brush, TerraformingMode carve)
        {
            _terraformingMode = carve;
            _brush = brush;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(SetBrushMode);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(SetBrushMode);
        }

        private void SetBrushMode()
        {
            _brush.SetModeClick(_terraformingMode);
        }
    }
}