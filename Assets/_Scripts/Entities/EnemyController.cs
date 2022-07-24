using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using _Scripts.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Entities
{
    public class EnemyController : BuffableEntity
    {

        [SerializeField] private float degreeDelta = 10f;
        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float movementSpeed = 5;
        
        [SerializeField] private int selectedWeaponId;
        [SerializeField] private int weaponLevel;
        
        [SerializeField] private GameObject tankCannon;
        [SerializeField] private HealthBarBehavior healthBar;

        protected override float MaxHealth => maxHealth;
        public override float MovementSpeed => movementSpeed;
        protected override GameObject TankCannon => tankCannon;
        protected override HealthBarBehavior HealthBar => healthBar;
        protected override SpriteRenderer CannonSr => _cannonSr;
        protected override SpriteRenderer MainSr => _sr;

        // TODO: Remove this?
        [SerializeField] public GameObject player;

        private int _xMovingDirection = 0;
        private bool _isAiming = false;
        private Vector2 _aimVelocity;
        
        private GameObject _projectilePrefab;
        private SpriteRenderer _sr, _cannonSr;
        private Rigidbody2D _projectileRigidbody2D, _rb2d;
        private LineRenderer _lr;
        
        void Start()
        {
            Health = maxHealth;
            HealthBar.SetHealth(Health, MaxHealth);
            
            _sr = GetComponent<SpriteRenderer>();
            _sr.flipX = FacingDirection == -1;

            _cannonSr = tankCannon.GetComponent<SpriteRenderer>();
            _cannonSr.flipX = FacingDirection == -1;
            
            _lr = GetComponent<LineRenderer>();

            _projectilePrefab = WeaponManager.Instance.GetWeaponById(selectedWeaponId).projectilePrefab;
            _projectileRigidbody2D = _projectilePrefab.GetComponent<Rigidbody2D>();

            _rb2d = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            CheckMovement();
            AdjustRotation();
            DrawTrajectory();
        }

        protected override void CheckMovement()
        {
            if (_xMovingDirection != 0)
            {
                transform.Translate(Time.deltaTime * movementSpeed * new Vector3(_xMovingDirection, 0, 0));
            }
            else
            {
                _rb2d.velocity = new Vector2(0, _rb2d.velocity.y);
            }
        }

        private void Aim()
        {
            _lr.enabled = true;
            _isAiming = true;
            
            Vector2 startPos = transform.position, endPos = player.transform.position;
            float t = 3f;

            float vx = (endPos.x - startPos.x) / t;
            float vy = (endPos.y - startPos.y + 0.5f * Physics2D.gravity.magnitude * t * t) / t;

            _aimVelocity = new Vector2(vx, vy);

            float rotateDegree = Random.Range(-degreeDelta, degreeDelta);
            
            _aimVelocity = Rotate(_aimVelocity, rotateDegree);
            
            SetCannonAngle(Vector2.SignedAngle(_aimVelocity, Vector2.right));
        }

        private void Shoot()
        {
            GameObject projectile = Instantiate(_projectilePrefab, gameObject.transform.position, transform.rotation);
            Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
            prb.velocity = _aimVelocity;
            
            LaunchedProjectile proj = projectile.GetComponent<LaunchedProjectile>();
            proj.Level = weaponLevel;

            _isAiming = false;
            gc.projectileShot = true;
        }
        

        public IEnumerator MakeMove()
        {

            // Simple Enemy AI 0.0.1
            // Initial Wait
            yield return new WaitForSeconds(1);
            
            // Randomly get the direction of going
            int random = Random.Range(1, 3);
            _xMovingDirection = random == 1 ? 1 : -1;

            // Flip if facing opposite direction
            if (_xMovingDirection != FacingDirection)
            {
                Flip();
            }
            
            // Walk for fixed second(s)
            yield return new WaitForSeconds(1);
            // Disable Walking
            _xMovingDirection = 0;

            // Aim and get the velocity
            Aim();
            // Aim for seconds
            yield return new WaitForSeconds(1);
            // Shoot projectile
            Shoot();
        }

        private void DrawTrajectory()
        {
            if (_isAiming)
            {
                Vector2[] trajectory = Plot(_projectileRigidbody2D, (Vector2)transform.position, _aimVelocity, 500);
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
        
        private Vector2 Rotate(Vector2 v, float delta)
        {
            float deltaRad = delta * Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(deltaRad) - v.y * Mathf.Sin(deltaRad),
                v.x * Mathf.Sin(deltaRad) + v.y * Mathf.Cos(deltaRad)
            );
        }
    }
}
