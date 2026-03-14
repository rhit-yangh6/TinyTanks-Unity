using System;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Projectiles;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Networking
{
    /// <summary>
    /// Networked aiming and firing system for multiplayer.
    /// Aiming is local-only (client-side preview).
    /// On release, sends ShootServerRpc → host spawns projectile as NetworkObject.
    /// </summary>
    public class MultiplayerLaunchProjectile : NetworkBehaviour
    {
        [SerializeField] public float cannonLength = 1;
        [SerializeField][Range(0f, 10f)] public float power = 5f;
        [SerializeField][Range(70f, 90f)] public float maxAngle = 80f;
        [SerializeField][Range(0f, 10.0f)] public float minAngle = 5f;
        [SerializeField][Range(1.0f, 3.0f)] public float aimProximity = 1.6f;

        private MultiplayerTankController _tank;
        private LineRenderer _lr;
        private Camera _cam;
        private Vector2 _startPoint;

        private int _selectedWeaponId;
        private int _selectedWeaponLevel;
        private GameObject _projectilePrefab;
        private LaunchedProjectile _selectedProjectile;
        private Rigidbody2D _prefabRb;
        private Weapon _selectedWeapon;
        private bool _needExtraForce, _isAiming;
        private float _extraForceXMultiplier, _extraForceYMultiplier;
        private float _cannonAngle;
        private GameEngine.WeaponExtraData.WeaponExtraData _wed;

        private static readonly int ShootAnim = Animator.StringToHash("Shoot");

        public bool IsAiming => _isAiming;

        private MultiplayerTankNetwork _tankNet;

        private void Start()
        {
            _cam = Camera.main;
            _lr = GetComponent<LineRenderer>();
            _tank = GetComponent<MultiplayerTankController>();
            _tankNet = GetComponent<MultiplayerTankNetwork>();
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (_tankNet == null) _tankNet = GetComponent<MultiplayerTankNetwork>();
            if (_tankNet == null || !_tankNet.IsMyTurn.Value) return;
            // Only allow input for the local player's tank
            if (_tankNet.PlayerIndex.Value != MultiplayerSessionData.LocalPlayerIndex) return;
            if (_selectedWeapon == null) return;

            var cannonPos = _tank.TankCannon.transform.position;
            var launchPos = LaunchProjectile.TrajectoryStartPositionHelper(
                _cannonAngle, cannonLength, cannonPos);

            // Right-click cancels aim
            if (Input.GetMouseButton(1) && _isAiming)
            {
                _lr.enabled = false;
                _isAiming = false;
            }

            if (Input.GetMouseButton(0) && (!_isAiming || _isAiming))
            {
                Vector2 dragPoint = _cam.ScreenToWorldPoint(Input.mousePosition);

                if (!_isAiming && Vector2.Distance(transform.position, dragPoint) < aimProximity)
                {
                    EventBus.Broadcast(EventTypes.StartedDragging);
                    _isAiming = true;
                    _lr.enabled = true;
                    _startPoint = dragPoint;
                }

                if (!_isAiming) return;

                Vector2 direction = (_startPoint - dragPoint).normalized;
                float magnitude = (_startPoint - dragPoint).magnitude;
                Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                // Local trajectory preview
                Vector2[] trajectory = Plot(_prefabRb, (Vector2)launchPos, velocity,
                    _selectedProjectile.GetSteps(), _needExtraForce);
                _lr.positionCount = trajectory.Length;
                Vector3[] positions = new Vector3[trajectory.Length];
                for (int i = 0; i < trajectory.Length; i++)
                    positions[i] = trajectory[i];
                _lr.SetPositions(positions);
            }
            else
            {
                if (_isAiming)
                {
                    _lr.enabled = false;
                    _isAiming = false;

                    EventBus.Broadcast(EventTypes.StoppedDragging);
                    Vector2 endPoint = _cam.ScreenToWorldPoint(Input.mousePosition);

                    Vector2 direction = (_startPoint - endPoint).normalized;
                    float magnitude = (_startPoint - endPoint).magnitude;
                    Vector2 velocity = CalculateFinalVelocity(direction, magnitude);

                    // Send to host to spawn projectile
                    ShootServerRpc(_selectedWeaponId, _selectedWeaponLevel,
                        velocity, _cannonAngle);
                }
            }
        }

        [ServerRpc]
        private void ShootServerRpc(int weaponId, int level, Vector2 velocity, float cannonAngle)
        {
            var weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            if (weapon == null)
            {
                Debug.LogError($"[MultiplayerLaunch] Unknown weapon ID: {weaponId}");
                return;
            }

            var tank = GetComponent<MultiplayerTankController>();
            var cannonPos = tank.TankCannon.transform.position;
            var launchPos = LaunchProjectile.TrajectoryStartPositionHelper(
                cannonAngle, cannonLength, cannonPos);

            // Spawn projectile on host
            GameObject projectile = Instantiate(weapon.projectilePrefab, launchPos, transform.rotation);
            Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
            prb.linearVelocity = velocity;

            LaunchedProjectile lp = projectile.GetComponent<LaunchedProjectile>();
            lp.SetParameters(weapon.damage, weapon.radius, weapon.maxMagnitude, weapon.steps);
            lp.Level = level;
            lp.Shooter = gameObject;
            lp.ShooterPlayerIndex = tank.GetComponent<MultiplayerTankNetwork>().PlayerIndex.Value;

            // Spawn as NetworkObject so clients see it
            var no = projectile.GetComponent<NetworkObject>();
            if (no != null)
            {
                no.Spawn();
            }

            // Trigger cannon animation on all clients
            PlayShootAnimClientRpc(cannonAngle);

            EventBus.Broadcast(EventTypes.ProjectileShot);
        }

        [ClientRpc]
        private void PlayShootAnimClientRpc(float cannonAngle)
        {
            _tank.SetCannonAngle(cannonAngle);
            var animator = _tank.TankCannon.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(ShootAnim);
            }
        }

        private Vector2 CalculateFinalVelocity(Vector2 direction, float magnitude)
        {
            var angle = Vector2.SignedAngle(direction, Vector2.right);
            var rotationAngle = gameObject.transform.eulerAngles.z;
            var angleAfterRotation = angle + rotationAngle;

            if (angleAfterRotation > 180)
                angleAfterRotation = -180 + (angleAfterRotation - 180);
            else if (angleAfterRotation < -180)
                angleAfterRotation = 180 - (angleAfterRotation + 180);

            bool isRight = Math.Abs(angleAfterRotation) < 90;
            Vector2 finalDirection;

            if (isRight && _tank.FacingDirection == -1)
                _tank.Flip();
            else if (!isRight && _tank.FacingDirection == 1)
                _tank.Flip();

            // Angle clamping (same logic as LaunchProjectile)
            if (angleAfterRotation > -minAngle || angleAfterRotation < -180 + minAngle)
            {
                finalDirection = Quaternion.Euler(0, 0, isRight ? minAngle + rotationAngle : -minAngle + rotationAngle) *
                                 (isRight ? Vector2.right : Vector2.left);
                _cannonAngle = isRight ? -minAngle - rotationAngle : -180 + minAngle - rotationAngle;
                _tank.SetCannonAngle(_cannonAngle);
            }
            else if (angleAfterRotation < -maxAngle && angleAfterRotation > -180 + maxAngle)
            {
                finalDirection = Quaternion.Euler(0, 0, isRight ? maxAngle + rotationAngle : -maxAngle + rotationAngle) *
                                 (isRight ? Vector2.right : Vector2.left);
                _cannonAngle = isRight ? -maxAngle - rotationAngle : -180 + maxAngle - rotationAngle;
                _tank.SetCannonAngle(_cannonAngle);
            }
            else
            {
                finalDirection = direction;
                _cannonAngle = angle;
                _tank.SetCannonAngle(angle);
            }

            var fixedMagnitude = _selectedProjectile.GetFixedMagnitude();
            if (fixedMagnitude < 0)
                return power * Math.Min(magnitude, _selectedProjectile.GetMaxMagnitude()) * finalDirection;
            return power * fixedMagnitude * finalDirection;
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

            float drag = 1f - timeStep * prb.linearDamping;
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

        public void SwitchWeapon(int weaponId, int level, GameEngine.WeaponExtraData.WeaponExtraData wed)
        {
            _selectedWeaponId = weaponId;
            _selectedWeaponLevel = level;
            _wed = wed;

            _selectedWeapon = WeaponManager.Instance.GetWeaponById(weaponId);
            _projectilePrefab = _selectedWeapon.projectilePrefab;
            _prefabRb = _projectilePrefab.GetComponent<Rigidbody2D>();
            _selectedProjectile = _projectilePrefab.GetComponent<LaunchedProjectile>();
            _selectedProjectile.Level = level;

            _needExtraForce = _projectilePrefab.GetComponent<ConstantForce2D>() != null;
            if (_needExtraForce)
            {
                _extraForceXMultiplier = Array.Find(_selectedWeapon.extraWeaponTerms,
                    ewt => ewt.term == "extraForceXMultiplier").value;
                _extraForceYMultiplier = Array.Find(_selectedWeapon.extraWeaponTerms,
                    ewt => ewt.term == "extraForceYMultiplier").value;
            }
        }
    }
}
