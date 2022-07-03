using System;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts
{
    public class LaunchProjectile : MonoBehaviour
    {
        public float power = 5f;
        public int selectedWeaponId;
        public GameObject player;
        public GameController gameController;
        public float maxAngle = 80f, minAngle = 5f;

        private PlayerController _playerCharacter;
        
        private LineRenderer _lr;

        private Camera _cam;
        private Vector2 startPoint, endPoint;

        private GameObject _projectilePrefab;
        private IProjectile _selectedProjectile;
        private Rigidbody2D _rb;

        private void Start()
        {
            _cam = Camera.main;
            _lr = GetComponent<LineRenderer>();

            _playerCharacter = player.GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && _playerCharacter.moveable)
            {
                Vector2 dragPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
            
                Vector2 direction = (startPoint - dragPoint).normalized;
                float magnitude = (startPoint - dragPoint).magnitude;

                Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                Vector2[] trajectory = Plot(_rb, (Vector2)transform.position, velocity, _selectedProjectile.getSteps());
                _lr.positionCount = trajectory.Length;

                Vector3[] positions = new Vector3[trajectory.Length];
                for (int i = 0; i < trajectory.Length; i++)
                {
                    positions[i] = trajectory[i];
                }
                _lr.SetPositions(positions);
            }
            else
            {
                _lr.enabled = false;
            }
        }

        private void OnMouseDown()
        {
            if (_playerCharacter.moveable)
            {
                startPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
                _lr.enabled = true;
            }
        }

        private void OnMouseUp()
        {
            if (_playerCharacter.moveable)
            {

                endPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        
                Vector2 direction = (startPoint - endPoint).normalized;
                float magnitude = (startPoint - endPoint).magnitude;
        
                Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                GameObject projectile = Instantiate(_projectilePrefab, gameObject.transform.position, transform.rotation);
                Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
                prb.velocity = velocity;

                gameController.projectileShot = true;
                _playerCharacter.moveable = false;
            }
        }

        private Vector2[] Plot(Rigidbody2D prb, Vector2 pos, Vector2 velocity, int steps)
        {
            Vector2[] results = new Vector2[steps];
            float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;

            Vector2 gravityAccel = prb.gravityScale * timestep * timestep * Physics2D.gravity;

            float drag = 1f - timestep * prb.drag;

            Vector2 moveStep = velocity * timestep;

            for (int i = 0; i < steps; i++)
            {
                moveStep += gravityAccel;
                moveStep *= drag;
                pos += moveStep;
                results[i] = pos;
            }

            return results;
        }

        private Vector2 CalculateFinalVelocity(Vector2 direction, float magnitude)
        {
            float angle = Vector2.SignedAngle(direction, Vector2.right);
            bool isRight = Math.Abs(angle) < 90;
            Vector2 finalDirection;

            if (isRight && _playerCharacter.facingDirection == -1)
            {
                _playerCharacter.Flip();
            } else if (!isRight && _playerCharacter.facingDirection == 1)
            {
                _playerCharacter.Flip();
            }

            // Angle Too Low
            if (angle > -minAngle || angle < -180 + minAngle)
            {
                finalDirection = Quaternion.Euler(0, 0, isRight ? minAngle : -minAngle) *
                                 (isRight ? Vector2.right : Vector2.left);
                _playerCharacter.SetCannonAngle(isRight ? minAngle : -180 + minAngle);
            } 
            // Angle Too High
            else if (angle < -maxAngle && angle > -180 + maxAngle)
            {
                finalDirection = Quaternion.Euler(0, 0, isRight ? maxAngle : -maxAngle) *
                                 (isRight ? Vector2.right : Vector2.left);
                _playerCharacter.SetCannonAngle(isRight ? maxAngle : -180 + maxAngle);
            }
            // Acceptable Angle
            else
            {
                finalDirection = direction;
                _playerCharacter.SetCannonAngle(angle);
            }
            
            return power * Math.Min(magnitude, _selectedProjectile.getMaxMagnitude()) * finalDirection;
        }

        public void RefreshPrefab()
        {
            _projectilePrefab = WeaponManager.Instance.GetWeaponById(selectedWeaponId).projectilePrefab;
            _rb = _projectilePrefab.GetComponent<Rigidbody2D>();
            _selectedProjectile = _projectilePrefab.GetComponent<IProjectile>();
        }
    }
}
