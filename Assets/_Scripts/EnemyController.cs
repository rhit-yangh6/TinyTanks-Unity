using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using _Scripts.Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class EnemyController : MonoBehaviour, Character
    {
        public float Health { get; set; }

        [SerializeField] public int facingDirection = -1;
        [SerializeField] public float degreeDelta = 10f;
        [SerializeField] public float maxHealth = 100;
        [SerializeField] public float movementSpeed = 5;
        [SerializeField] public int selectedWeaponId;
        [SerializeField] private int weaponLevel;
        [SerializeField] public GameController gameController;
        [SerializeField] public GameObject tankCannon;
        [SerializeField] public HealthBarBehavior healthBar;
        [SerializeField] private LayerMask layerMask;
        
        // TODO: Remove this?
        [SerializeField] public GameObject player;
        
        public bool isDead = false;
        
        private int _xMovingDirection = 0;
        private bool _isAiming = false;
        private Vector2 _aimVelocity;
        
        private GameObject _projectilePrefab;
        private SpriteRenderer _sr, _cannonSr;
        private Rigidbody2D _projectileRigidbody2D, _rb2d;
        private LineRenderer _lr;
        
        private readonly Dictionary<ScriptableBuff, TimedBuff> _buffs = new ();

        void Start()
        {
            Health = maxHealth;
            healthBar.SetHealth(Health, maxHealth);
            
            _sr = GetComponent<SpriteRenderer>();
            _sr.flipX = facingDirection == -1;

            _cannonSr = tankCannon.GetComponent<SpriteRenderer>();
            _cannonSr.flipX = facingDirection == -1;
            
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
    
        public void TakeDamage(float amount)
        {
            if (Health - amount <= 0)
            {
                Health = 0;
                isDead = true;
                Destroy(gameObject);
            }
            else
            {
                Health -= amount;
            }
            healthBar.SetHealth(Health, maxHealth);
        }

        public void AdjustRotation()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 3f, layerMask);

            if (hit.collider)
            {
                float angle = Vector2.SignedAngle(hit.normal, Vector2.up);
                transform.eulerAngles = new Vector3 (0, 0, -angle);
            }
            else
            {
                transform.eulerAngles = new Vector3 (0, 0, 0);
            }
        }

        public void CheckMovement()
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

        public void Flip()
        {
            facingDirection *= -1;
            _sr.flipX = facingDirection == -1;
            _cannonSr.flipX = facingDirection == -1;
        }

        public void SetCannonAngle(float angle)
        {
            tankCannon.transform.localEulerAngles = (facingDirection == 1 ? -angle : (180 - angle)) * Vector3.forward;
        }

        public void AddBuff(TimedBuff buff)
        {
            if (_buffs.ContainsKey(buff.Buff))
            {
                _buffs[buff.Buff].Activate();
            }
            else
            {
                _buffs.Add(buff.Buff, buff);
                buff.Activate();
            }
        }

        public void TickBuffs()
        {
            foreach (var buff in _buffs.Values.ToList())
            {
                buff.Tick();
                if (buff.isFinished)
                {
                    _buffs.Remove(buff.Buff);
                }
            }
        }

        public void IncreaseMovementSpeed(float amount)
        {
            movementSpeed += amount;
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
            gameController.projectileShot = true;
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
            if (_xMovingDirection != facingDirection)
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
