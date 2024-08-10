using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Projectiles;
using _Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Entities
{
    public class EnemyController : BuffableEntity
    {
        [SerializeField] protected float degreeDeltaMax = 25f;
        [SerializeField] protected float degreeDeltaMin = 2f;
        [SerializeField] protected int selectedWeaponId;
        [SerializeField] protected int weaponLevel;
        [SerializeField] protected float horizontalCastDistance = 1f;
        [SerializeField] protected float verticalCastDistance = 1f;
        [SerializeField] protected float cannonLength = 1f;
        [SerializeField] protected float approachTargetTendency = 0.5f;
        [SerializeField][Range(40f, 180f)] protected float climbAngleTolerance = 70f;
        
        private const float DegreeDeltaMaxDistance = 40f;
        private const float DegreeDeltaMinDistance = 5f;
        
        // The projectilePrefab of the selected weapon, applied to enemies
        protected GameObject ProjectilePrefab;
        
        // Line Renderer
        protected LineRenderer LineRenderer;
        
        // If the enemy is aiming
        protected bool IsAiming = false;
        
        // Aim Velocity of projectiles
        protected Vector2 AimVelocity;
        
        // Moving direction of enemies
        protected int XMovingDirection;
        
        // Rotation angle of enemy cannon
        protected float CannonAngle;
        
        // Rotation angle of enemy cannon
        protected GameObject TargetObject;
        
        protected override void Start()
        {
            base.Start();
            ProjectilePrefab = WeaponManager.Instance.GetWeaponById(selectedWeaponId).projectilePrefab;
            LineRenderer = GetComponent<LineRenderer>();
        }

        private void FixedUpdate()
        {
            CheckMovement();
            AdjustRotation();
            DrawTrajectory();
        }

        protected override void CheckMovement()
        {
            SlopeCheckHorizontal();
            EdgeCheckVertical();
            if (XMovingDirection != 0)
            {
                if (IsGrounded())
                {
                    // _rb2d.velocity = movementSpeed * new Vector3(_xMovingDirection, 0, 0);
                    transform.Translate(Time.deltaTime * movementSpeed * new Vector3(XMovingDirection, 0, 0));
                }
                else
                {
                    // _rb2d.velocity = movementSpeed * new Vector3(_xMovingDirection, -1, 0);
                    transform.Translate(Time.deltaTime * movementSpeed * new Vector3(XMovingDirection, -1, 0));
                }
                // Rigidbody2D.velocity = new Vector2(XMovingDirection * movementSpeed, Rigidbody2D.velocity.y);
            }
            else
            {
                // _rb2d.velocity = new Vector2(0, _rb2d.velocity.y);
                if (IsGrounded())
                {
                    Rigidbody2D.velocity = Vector2.zero;
                    Rigidbody2D.isKinematic = true;
                }
                else
                {
                    Rigidbody2D.isKinematic = false;
                }
            }
        }

        private void SlopeCheckHorizontal()
        {
            // Raycast in two horizontal directions. If the angle is too high stop moving immediately
            Vector2 Checkpos = transform.position - new Vector3(0f, ColliderSize.y / 3);
            var slopeHitRight = Physics2D.Raycast(Checkpos, Vector2.right, horizontalCastDistance, layerMask);
            var slopeHitLeft = Physics2D.Raycast(Checkpos, Vector2.left, horizontalCastDistance, layerMask);
            // Debug.DrawRay(Checkpos, Vector2.right, Color.green, 10);
            // Debug.DrawRay(Checkpos, Vector2.left, Color.red, 10);

            var slopeSideAngle = XMovingDirection switch
            {
                1 => Vector2.Angle(slopeHitRight.normal, Vector2.up),
                -1 => Vector2.Angle(slopeHitLeft.normal, Vector2.up),
                _ => 0
            };
            
            // Debug.Log(XMovingDirection + "---" + slopeSideAngle);
            
            if (slopeHitRight && XMovingDirection > 0 && slopeSideAngle > climbAngleTolerance)
            {
                XMovingDirection = 0;
            } else if (slopeHitLeft && XMovingDirection < 0 && slopeSideAngle > climbAngleTolerance)
            {
                XMovingDirection = 0;
            }
        }

        private void EdgeCheckVertical()
        {
            var pos = transform.position;
            Vector2 leftCheckPos = pos - new Vector3(ColliderSize.x / 2, ColliderSize.y / 3);
            Vector2 rightCheckPos = pos - new Vector3(-ColliderSize.x / 2, ColliderSize.y / 3);
            // Debug.DrawRay(leftCheckPos, Vector2.down, Color.green, 10);
            // Debug.DrawRay(rightCheckPos, Vector2.down, Color.red, 10);
            
            var hitLeft = Physics2D.Raycast(leftCheckPos, Vector2.down, verticalCastDistance, layerMask);
            var hitRight = Physics2D.Raycast(rightCheckPos, Vector2.down, verticalCastDistance, layerMask);

            switch (XMovingDirection)
            {
                // Moving right but not seeing floor
                case 1 when !hitRight:
                case -1 when !hitLeft:
                    XMovingDirection = 0;
                    break;
            }
        }
        
        public virtual IEnumerator MakeMove()
        {
           // Empty AI
           // Enemies do nothing by default
           yield return 0;
        }
        
        // Override functions
        protected override void OnDeath()
        {
            base.OnDeath();
            // SteamManager.IncrementStat(Constants.StatBasicEnemiesKilled);
        }
        
        // Private functions
        protected void DrawTrajectory()
        {
            if (IsAiming)
            {
                var launchPos =
                    LaunchProjectile.TrajectoryStartPositionHelper(CannonAngle, cannonLength,
                        tankCannon.transform.position);
                
                Vector2[] trajectory = Plot(ProjectilePrefab.GetComponent<Rigidbody2D>(), (Vector2)launchPos, AimVelocity, 500);
                LineRenderer.positionCount = trajectory.Length;

                Vector3[] positions = new Vector3[trajectory.Length];
                for (int i = 0; i < trajectory.Length; i++)
                {
                    positions[i] = trajectory[i];
                }
                LineRenderer.SetPositions(positions);
            }
            else
            {
                LineRenderer.enabled = false;
            }
        }
        
        protected Vector2[] Plot(Rigidbody2D prb, Vector2 pos, Vector2 velocity, int steps)
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

        protected void Shoot()
        {
            var launchPos =
                LaunchProjectile.TrajectoryStartPositionHelper(CannonAngle, cannonLength,
                    tankCannon.transform.position);
            tankCannon.GetComponent<Animator>().SetTrigger(Shoot1);

            var projectile = Instantiate(ProjectilePrefab, launchPos, transform.rotation);
            var prb = projectile.GetComponent<Rigidbody2D>();
            prb.velocity = AimVelocity;

            var proj = projectile.GetComponent<LaunchedProjectile>();
            var w = WeaponManager.Instance.GetWeaponById(selectedWeaponId);
            proj.SetParameters(w.damage, w.radius, w.maxMagnitude, w.steps);
            proj.Level = weaponLevel;
            proj.Shooter = gameObject;

            IsAiming = false;
            EventBus.Broadcast(EventTypes.ProjectileShot);
        }

        protected void Aim()
        {
            LineRenderer.enabled = true;
            IsAiming = true;
            var targetPosition = TargetObject.transform.position;
            var aimingDirection = targetPosition.x < transform.position.x ? -1 : 1;
            if (aimingDirection != FacingDirection)
            {
                Flip();
            }
            
            Vector2 startPos =
                LaunchProjectile.TrajectoryStartPositionHelper(CannonAngle, cannonLength,
                    tankCannon.transform.position);
            
            float t = 3f;

            float vx = (targetPosition.x - startPos.x) / t;
            float vy = (targetPosition.y - startPos.y + 0.5f * Physics2D.gravity.magnitude * t * t) / t;

            AimVelocity = new Vector2(vx, vy);

            // Calculate the deviation angle depending on the distance to its target
            var distance = Vector2.Distance(targetPosition, transform.position);
            var finalDegreeDelta = distance switch
            {
                < DegreeDeltaMinDistance => degreeDeltaMin,
                > DegreeDeltaMaxDistance => degreeDeltaMax,
                _ => (degreeDeltaMax - degreeDeltaMin) / (DegreeDeltaMaxDistance - DegreeDeltaMinDistance) *
                    (distance - DegreeDeltaMinDistance) + degreeDeltaMin
            };
            var rotateDegree = Random.Range(-finalDegreeDelta, finalDegreeDelta);
            
            AimVelocity = Geometry.Rotate(AimVelocity, rotateDegree);
            CannonAngle = Vector2.SignedAngle(AimVelocity, Vector2.right);
            SetCannonAngle(CannonAngle);
        }

        // Locate the target that it is attacking
        protected GameObject FindTarget()
        {
            return GameObject.FindGameObjectWithTag("Player");
        }
    }
}
