using UnityEngine;

namespace ExplosionGame
{
    public class CameraSizeFit : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _sceneWidth = 10;

        private void Start()
        {
            float unitsPerPixel = _sceneWidth / Screen.width;

            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            _camera.orthographicSize = desiredHalfHeight;
        }
    }
}