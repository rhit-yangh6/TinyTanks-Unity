using System;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.GameEngine
{
    public class LaunchProjectile : MonoBehaviour
    {
        public GameObject player;
        [SerializeField] public float cannonLength = 1;
        [SerializeField][Range(0f, 10f)] public float power = 5f;
        [SerializeField][Range(70f, 90f)] public float maxAngle = 80f;
        [SerializeField][Range(0f, 10.0f)] public float minAngle = 5f;
        [SerializeField][Range(1.0f, 3.0f)] public float aimProximity = 1.6f;

        private PlayerController _playerCharacter;
        
        private LineRenderer _lr;

        private Camera _cam;
        private Vector2 _startPoint, _endPoint;

        private SelectionDatum _sd;
        private GameObject _projectilePrefab;
        private LaunchedProjectile _selectedProjectile;
        private Rigidbody2D _rb;
        private Weapon _selectedWeapon;
        private bool _needExtraForce, _isAiming;
        private float _extraForceXMultiplier, _extraForceYMultiplier;
        private float _cannonAngle;
        private static readonly int Shoot = Animator.StringToHash("Shoot");

        private void Start()
        {
            _cam = Camera.main;
            _lr = GetComponent<LineRenderer>();

            _playerCharacter = player.GetComponent<PlayerController>();
        }

        private void Update()
        {
            var launchPos = TrajectoryStartPositionHelper(_cannonAngle, cannonLength,
                _playerCharacter.tankCannon.transform.position);
            if (Input.GetMouseButton(1) && _isAiming)
            {
                _lr.enabled = false;
                _isAiming = false;
                _playerCharacter.moveable = true;
            }
            
            if (Input.GetMouseButton(0) && (_playerCharacter.moveable || _isAiming))
            {
                Vector2 dragPoint = _cam.ScreenToWorldPoint(Input.mousePosition);

                if (!_isAiming && Vector2.Distance(transform.position, dragPoint) < aimProximity)
                {
                    EventBus.Broadcast(EventTypes.StartedDragging);
                    _isAiming = true;
                    _lr.enabled = true;
                    _playerCharacter.moveable = false;
                    _startPoint = dragPoint;
                }

                if (!_isAiming) return; // Not in proximity, return
            
                Vector2 direction = (_startPoint - dragPoint).normalized;
                float magnitude = (_startPoint - dragPoint).magnitude;

                Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                
                Vector2[] trajectory = Plot(_rb, (Vector2)launchPos, velocity, _selectedProjectile.GetSteps(), _needExtraForce);
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
                if (_isAiming)
                {
                    _lr.enabled = false;
                    _isAiming = false;
                    _playerCharacter.moveable = true;
                    
                    EventBus.Broadcast(EventTypes.StoppedDragging);
                    _endPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
        
                    Vector2 direction = (_startPoint - _endPoint).normalized;
                    float magnitude = (_startPoint - _endPoint).magnitude;
        
                    Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                    GameObject projectile = Instantiate(_projectilePrefab, launchPos, transform.rotation);
                    Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
                    prb.velocity = velocity;
                
                    LaunchedProjectile lp = projectile.GetComponent<LaunchedProjectile>();
                    lp.SetParameters(_selectedWeapon.damage, _selectedWeapon.radius, _selectedWeapon.maxMagnitude, _selectedWeapon.steps);
                    lp.Level = _sd.level;
                    lp.Shooter = gameObject;

                    _playerCharacter.tankCannon.GetComponent<Animator>().SetTrigger(Shoot);
                
                    EventBus.Broadcast(EventTypes.ProjectileShot);
                    _playerCharacter.moveable = false;
                }
            }
        }

        private Vector2[] Plot(Rigidbody2D prb, Vector2 pos, Vector2 velocity, int steps, bool needExtraForce)
        {
            var results = new Vector2[steps];
            var timeStep = Time.fixedDeltaTime / Physics2D.velocityIterations;
            
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
                _cannonAngle = isRight ? -minAngle - rotationAngle : -180 + minAngle - rotationAngle;
                _playerCharacter.SetCannonAngle(_cannonAngle);
            } 
            // Angle Too High
            else if (angleAfterRotation < -maxAngle && angleAfterRotation > -180 + maxAngle)
            {
                finalDirection = Quaternion.Euler(0, 0, isRight ? maxAngle + rotationAngle : -maxAngle + rotationAngle) *
                                 (isRight ? Vector2.right : Vector2.left);
                _cannonAngle = isRight ? -maxAngle - rotationAngle : -180 + maxAngle - rotationAngle;
                _playerCharacter.SetCannonAngle(_cannonAngle);
            }
            // Acceptable Angle
            else
            {
                finalDirection = direction;
                _cannonAngle = angle;
                _playerCharacter.SetCannonAngle(angle);
            }
            
            var fixedMagnitude = _selectedProjectile.GetFixedMagnitude();
            if (fixedMagnitude < 0)
            {
                return power * Math.Min(magnitude, _selectedProjectile.GetMaxMagnitude()) * finalDirection;
            }

            return power * fixedMagnitude * finalDirection;
        }

        public void SwitchWeapon(SelectionDatum sd)
        {
            _sd = sd;

            _selectedWeapon = WeaponManager.Instance.GetWeaponById(sd.weaponId);
            
            _projectilePrefab = _selectedWeapon.projectilePrefab;
            _rb = _projectilePrefab.GetComponent<Rigidbody2D>();
            _selectedProjectile = _projectilePrefab.GetComponent<LaunchedProjectile>();
            
            _selectedProjectile.GetComponent<LaunchedProjectile>().Level = sd.level;
            
            _needExtraForce = _projectilePrefab.GetComponent<ConstantForce2D>() != null;

            if (_needExtraForce)
            {
                _extraForceXMultiplier = Array.Find(_selectedWeapon.extraWeaponTerms, ewt => ewt.term == "extraForceXMultiplier").value;
                _extraForceYMultiplier = Array.Find(_selectedWeapon.extraWeaponTerms, ewt => ewt.term == "extraForceYMultiplier").value;
            }
        }

        public static Vector3 TrajectoryStartPositionHelper(float cannonAngle, float cannonLength, Vector3 cannonPos)
        {
            var cannonAngleRad = (cannonAngle * Math.PI) / 180;
            return cannonPos + new Vector3((float)Math.Cos(cannonAngleRad) * cannonLength,
                                (float)-Math.Sin(cannonAngleRad) * cannonLength, 0);
        }
    }
}
