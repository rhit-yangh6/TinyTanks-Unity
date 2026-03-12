using _Scripts.GameEngine;
using UnityEngine;

namespace _Scripts.Networking
{
    /// <summary>
    /// Turn-aware camera for multiplayer mode.
    /// - Local player's turn: free camera centered on own tank, with mouse edge pan + zoom.
    /// - Other player's turn: auto-follow active player, manual override allowed.
    /// - On projectile shot: follow projectile.
    /// - On turn transition: smooth pan to next player.
    /// - Respects world boundaries via CameraBoundingBox.
    /// </summary>
    public class MultiplayerCameraController : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float panEdgeSize = 30f;
        [SerializeField] private float zoomStep = 1f;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 15f;
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float transitionSpeed = 3f;

        private Camera _cam;
        private Transform _followTarget;
        private bool _isTransitioning;
        private Vector3 _transitionTarget;
        private bool _manualOverride;
        private MultiplayerGameController _gameController;

        // World bounds
        private float _mapMinX, _mapMaxX, _mapMinY, _mapMaxY;
        private Bounds _bounds;
        private bool _hasBounds;

        private void Start()
        {
            _cam = Camera.main;

            _gameController = FindAnyObjectByType<MultiplayerGameController>();
            if (_gameController != null)
            {
                _gameController.OnTurnChanged += OnTurnChanged;
            }

            InitBounds();
        }

        private void InitBounds()
        {
            var boundsObj = GameObject.FindGameObjectWithTag("CameraBoundingBox");
            if (boundsObj == null) return;

            var boundsSr = boundsObj.GetComponent<SpriteRenderer>();
            if (boundsSr == null) return;

            _hasBounds = true;
            _bounds = boundsSr.bounds;
            var pos = boundsSr.transform.position;

            _mapMinX = pos.x - _bounds.size.x / 2f;
            _mapMaxX = pos.x + _bounds.size.x / 2f;
            _mapMinY = pos.y - _bounds.size.y / 2f;
            _mapMaxY = pos.y + _bounds.size.y / 2f;

            // Clamp initial zoom if it exceeds bounds
            ClampZoomToBounds();
        }

        private void OnDestroy()
        {
            if (_gameController != null)
            {
                _gameController.OnTurnChanged -= OnTurnChanged;
            }
        }

        private void LateUpdate()
        {
            if (_cam == null) return;

            HandleZoom();

            if (_isTransitioning)
            {
                HandleTransition();
                ClampToBounds();
                return;
            }

            // Check for projectile to follow
            var projectile = GameObject.FindGameObjectWithTag("AggressiveProjectile");
            if (projectile != null)
            {
                _followTarget = projectile.transform;
                _manualOverride = false;
            }

            // Edge panning (manual override)
            if (HandleEdgePan())
            {
                _manualOverride = true;
                ClampToBounds();
                return;
            }

            // Follow target if not manually overriding
            if (!_manualOverride && _followTarget != null)
            {
                FollowTarget();
            }

            ClampToBounds();
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) < 0.01f) return;

            if (scroll > 0)
            {
                _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize - zoomStep, minZoom, maxZoom);
            }
            else
            {
                // Don't zoom out past bounds
                float newSize = _cam.orthographicSize + zoomStep;
                if (_hasBounds)
                {
                    float maxH = _bounds.size.y / 2f;
                    float maxW = _bounds.size.x / (2f * _cam.aspect);
                    newSize = Mathf.Min(newSize, Mathf.Min(maxH, maxW));
                }
                _cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }

        private void ClampZoomToBounds()
        {
            if (!_hasBounds || _cam == null) return;

            float maxH = _bounds.size.y / 2f;
            float maxW = _bounds.size.x / (2f * _cam.aspect);
            float clampedMax = Mathf.Min(maxH, maxW);

            if (_cam.orthographicSize > clampedMax)
                _cam.orthographicSize = clampedMax;
        }

        private bool HandleEdgePan()
        {
            Vector3 pos = _cam.transform.position;
            bool panned = false;

            if (Input.mousePosition.x >= Screen.width - panEdgeSize)
            {
                pos.x += panSpeed * Time.deltaTime;
                panned = true;
            }
            else if (Input.mousePosition.x <= panEdgeSize)
            {
                pos.x -= panSpeed * Time.deltaTime;
                panned = true;
            }

            if (Input.mousePosition.y >= Screen.height - panEdgeSize)
            {
                pos.y += panSpeed * Time.deltaTime;
                panned = true;
            }
            else if (Input.mousePosition.y <= panEdgeSize)
            {
                pos.y -= panSpeed * Time.deltaTime;
                panned = true;
            }

            if (panned)
            {
                _cam.transform.position = pos;
            }

            return panned;
        }

        private void FollowTarget()
        {
            if (_followTarget == null) return;

            Vector3 target = new Vector3(
                _followTarget.position.x,
                _followTarget.position.y,
                _cam.transform.position.z);

            _cam.transform.position = Vector3.Lerp(
                _cam.transform.position, target, followSpeed * Time.deltaTime);
        }

        private void HandleTransition()
        {
            _cam.transform.position = Vector3.Lerp(
                _cam.transform.position, _transitionTarget, transitionSpeed * Time.deltaTime);

            if (Vector3.Distance(_cam.transform.position, _transitionTarget) < 0.1f)
            {
                _isTransitioning = false;
            }
        }

        private void ClampToBounds()
        {
            if (!_hasBounds) return;

            var camHeight = _cam.orthographicSize;
            var camWidth = camHeight * _cam.aspect;

            var minX = _mapMinX + camWidth;
            var maxX = _mapMaxX - camWidth;
            var minY = _mapMinY + camHeight;
            var maxY = _mapMaxY - camHeight;

            var pos = _cam.transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            _cam.transform.position = pos;
        }

        private void OnTurnChanged(int playerIndex)
        {
            if (_gameController == null) return;

            var tanks = _gameController.Tanks;
            if (playerIndex < tanks.Count && tanks[playerIndex] != null)
            {
                _followTarget = tanks[playerIndex].transform;
                _manualOverride = false;

                // Smooth transition to new player
                _transitionTarget = new Vector3(
                    _followTarget.position.x,
                    _followTarget.position.y,
                    _cam.transform.position.z);
                _isTransitioning = true;
            }
        }

        /// <summary>
        /// Force camera to center on a specific position.
        /// </summary>
        public void CenterOn(Vector3 position)
        {
            _transitionTarget = new Vector3(position.x, position.y, _cam.transform.position.z);
            _isTransitioning = true;
            _manualOverride = false;
        }
    }
}
