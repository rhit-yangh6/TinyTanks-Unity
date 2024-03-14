using System;
using _Scripts.Entities;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts
{
    public class LaunchProjectile : MonoBehaviour
    {
        public float power = 5f;
        public GameObject player;
        public float maxAngle = 80f, minAngle = 5f;

        private PlayerController _playerCharacter;
        
        private LineRenderer _lr;

        private Camera _cam;
        private Vector2 _startPoint, _endPoint;

        private GameController _gc;
        private SelectionDatum _sd;
        private GameObject _projectilePrefab;
        private LaunchedProjectile _selectedProjectile;
        private Rigidbody2D _rb;
        private bool _needExtraForce;
        private float _extraForceXMultiplier, _extraForceYMultiplier;

        private void Start()
        {
            _cam = Camera.main;
            _lr = GetComponent<LineRenderer>();
            _gc = GameObject.FindGameObjectWithTag("GC").GetComponent<GameController>();

            _playerCharacter = player.GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && _playerCharacter.moveable)
            {
                Vector2 dragPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
            
                Vector2 direction = (_startPoint - dragPoint).normalized;
                float magnitude = (_startPoint - dragPoint).magnitude;

                Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                Vector2[] trajectory = Plot(_rb, (Vector2)transform.position, velocity, _selectedProjectile.GetSteps(), _needExtraForce);
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
                _startPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
                _lr.enabled = true;
            }
        }

        private void OnMouseUp()
        {
            if (_playerCharacter.moveable)
            {

                _endPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        
                Vector2 direction = (_startPoint - _endPoint).normalized;
                float magnitude = (_startPoint - _endPoint).magnitude;
        
                Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                GameObject projectile = Instantiate(_projectilePrefab, gameObject.transform.position, transform.rotation);
                Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
                prb.velocity = velocity;
                
                LaunchedProjectile lp = projectile.GetComponent<LaunchedProjectile>();
                lp.Level = _sd.level;
                lp.Shooter = gameObject;

                _gc.projectileShot = true;
                _playerCharacter.moveable = false;
            }
        }

        private Vector2[] Plot(Rigidbody2D prb, Vector2 pos, Vector2 velocity, int steps, bool needExtraForce)
        {
            Vector2[] results = new Vector2[steps];
            float timeStep = Time.fixedDeltaTime / Physics2D.velocityIterations;
            
            Vector2 gravityAccel;
            if (needExtraForce)
            {
                gravityAccel = prb.gravityScale * timeStep * timeStep * 
                               (Physics2D.gravity + new Vector2(
                                   velocity.x * _extraForceXMultiplier,
                                   velocity.y * _extraForceYMultiplier));
            }
            else
            {
                gravityAccel = prb.gravityScale * timeStep * timeStep * Physics2D.gravity;
            }

            float drag = 1f - timeStep * prb.drag;

            Vector2 moveStep = velocity * timeStep;

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
            var angle = Vector2.SignedAngle(direction, Vector2.right);
            var rotationAngle = gameObject.transform.eulerAngles.z;

            var angleAfterRotation = angle + rotationAngle;

            if (angleAfterRotation > 180)
            {
                angleAfterRotation = -180 + (angleAfterRotation - 180);
            }
            else if (angleAfterRotation < -180)
            {
                angleAfterRotation = 180 - (angleAfterRotation + 180);
            }

            bool isRight = Math.Abs(angleAfterRotation) < 90;
            Vector2 finalDirection;
            // Debug.Log(angleAfterRotation);
            
            if (isRight && _playerCharacter.FacingDirection == -1)
            {
                _playerCharacter.Flip();
            } else if (!isRight && _playerCharacter.FacingDirection == 1)
            {
                _playerCharacter.Flip();
            }

            // Angle Too Low
            if (angleAfterRotation > -minAngle || angleAfterRotation < -180 + minAngle)
            {
                finalDirection = Quaternion.Euler(0, 0, isRight ? minAngle + rotationAngle : -minAngle + rotationAngle) *
                                 (isRight ? Vector2.right : Vector2.left);
                _playerCharacter.SetCannonAngle(isRight ? -minAngle - rotationAngle : -180 + minAngle - rotationAngle);
            } 
            // Angle Too High
            else if (angleAfterRotation < -maxAngle && angleAfterRotation > -180 + maxAngle)
            {
                finalDirection = Quaternion.Euler(0, 0, isRight ? maxAngle + rotationAngle : -maxAngle + rotationAngle) *
                                 (isRight ? Vector2.right : Vector2.left);
                _playerCharacter.SetCannonAngle(isRight ? -maxAngle - rotationAngle : -180 + maxAngle - rotationAngle);
            }
            // Acceptable Angle
            else
            {
                finalDirection = direction;
                _playerCharacter.SetCannonAngle(angle);
            }
            
            float fixedMagnitude = _selectedProjectile.GetFixedMagnitude();
            if (fixedMagnitude < 0)
            {
                return power * Math.Min(magnitude, _selectedProjectile.GetMaxMagnitude()) * finalDirection;
            }
            else
            {
                return power * fixedMagnitude * finalDirection;

            }
        }

        public void SwitchWeapon(SelectionDatum sd)
        {
            _sd = sd;

            Weapon w = WeaponManager.Instance.GetWeaponById(sd.weaponId);
            
            _projectilePrefab = w.projectilePrefab;
            _rb = _projectilePrefab.GetComponent<Rigidbody2D>();
            _selectedProjectile = _projectilePrefab.GetComponent<LaunchedProjectile>();
            
            _selectedProjectile.GetComponent<LaunchedProjectile>().Level = sd.level;
            
            _needExtraForce = _projectilePrefab.GetComponent<ConstantForce2D>() != null;

            if (_needExtraForce)
            {
                _extraForceXMultiplier = Array.Find(w.extraWeaponTerms, ewt => ewt.term == "extraForceXMultiplier").value;
                _extraForceYMultiplier = Array.Find(w.extraWeaponTerms, ewt => ewt.term == "extraForceYMultiplier").value;
            }
            
        }
    }
}
