using UnityEngine;
using UnityEngine.UI;

namespace DemosShared
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class UISwitchButton : MonoBehaviour
    {
        [SerializeField] private RectTransform _ui;
        [SerializeField] private Button _button;

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(ToggleUI);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(ToggleUI);
        }

        private void ToggleUI()
        {
            bool state = _ui.gameObject.activeInHierarchy == false;
            _ui.gameObject.SetActive(state);
        }
    }
}