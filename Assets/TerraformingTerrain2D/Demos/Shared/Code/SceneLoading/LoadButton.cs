using UnityEngine;
using UnityEngine.UI;

namespace DemosShared
{
    [RequireComponent(typeof(Button))]
    public class LoadButton : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private Button _button;
        private ISceneLoading _sceneLoading;
        
        private void OnValidate()
        {
            _button ??= GetComponent<Button>();
        }

        private void Awake()
        {
            _sceneLoading = SceneLoadingProvider.GetSceneLoading();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(LoadDemo);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(LoadDemo);
        }

        private void LoadDemo()
        {
            _sceneLoading.Load(_sceneName);
        }
    }
}