using _Scripts.GameEngine;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float zoomStep, minCamSize, maxCamSize, moveStep;
        [SerializeField] private float shakeAmount, shakeDecreaseFactor;
        [SerializeField] private SpriteRenderer boundsSr;
        
        private Vector3 _dragOrigin, _originalPos;
        private float _shakeDuration;
        private bool _isShaking;

        private float _mapMinX, _mapMinY, _mapMaxX, _mapMaxY;
        
        private void Awake()
        {
            // Only load prefab in Story Mode
            if (SceneManager.GetActiveScene().name == "Story")
            {
                var currentLevel = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId);
                Instantiate(Resources.Load<GameObject>("LevelPrefabs/" + currentLevel.path));
                boundsSr = GameObject.FindGameObjectWithTag("CameraBoundingBox").GetComponent<SpriteRenderer>();
            }
            var pos = boundsSr.transform.position;
            var bounds = boundsSr.bounds;
            
            _mapMinX = pos.x - bounds.size.x / 2f;
            _mapMaxX = pos.x + bounds.size.x / 2f;
            
            _mapMinY = pos.y - bounds.size.y / 2f;
            _mapMaxY = pos.y + bounds.size.y / 2f;
        }

        private void Update()
        {
            MoveCamera();
            ZoomCamera();

            if (_isShaking)
            {
                Shake();
            }
            
            cam.transform.position = ClampCamera(cam.transform.position);
        }

        private void MoveCamera()
        {
            if ( Input.mousePosition.x <= Screen.width * 0.04) 
                cam.transform.Translate(Time.deltaTime * moveStep * Vector3.left, Space.World);
            if (Input.mousePosition.x >= Screen.width * 0.96) 
                cam.transform.Translate(Time.deltaTime * moveStep * Vector3.right, Space.World);
            if (Input.mousePosition.y >= Screen.height * 0.96) 
                cam.transform.Translate(Time.deltaTime * moveStep * Vector3.up, Space.World);
            if (Input.mousePosition.y <= Screen.height * 0.04) 
                cam.transform.Translate(Time.deltaTime * moveStep * Vector3.down, Space.World);
        }

        private void ZoomCamera()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0) ZoomIn();
            if (Input.GetAxis("Mouse ScrollWheel") < 0) ZoomOut();
        }

        private void ZoomIn()
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomStep, minCamSize, maxCamSize);
        }

        private void ZoomOut()
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomStep, minCamSize, maxCamSize);
        }

        public void ShakeCamera(float duration)
        {
            _originalPos = cam.transform.localPosition;
            _isShaking = true;
            _shakeDuration = duration;
        }

        private void Shake()
        {
            if (_shakeDuration > 0)
            {
                cam.transform.localPosition = _originalPos + Random.insideUnitSphere * shakeAmount;
                _shakeDuration -= Time.deltaTime * shakeDecreaseFactor;    
            }
            else
            {
                _isShaking = false;
                _shakeDuration = 0f;
                cam.transform.localPosition = _originalPos;
            }
        }

        private Vector3 ClampCamera(Vector3 targetPosition)
        {
            
            var camHeight = cam.orthographicSize;
            var camWidth = camHeight * cam.aspect;

            var minX = _mapMinX + camWidth;
            var maxX = _mapMaxX - camWidth;
            var minY = _mapMinY + camHeight;
            var maxY = _mapMaxY - camHeight;

            var newX = Mathf.Clamp(targetPosition.x, minX, maxX);
            var newY = Mathf.Clamp(targetPosition.y, minY, maxY);

            return new Vector3(newX, newY, targetPosition.z);
        }
    }
}
