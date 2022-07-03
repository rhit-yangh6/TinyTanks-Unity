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

        private PlayerController playerCharacter;
        
        private LineRenderer lr;

        private Camera cam;
        private Vector2 startPoint, endPoint;

        private GameObject _projectilePrefab;
        private IProjectile _selectedProjectile;
        private Rigidbody2D _rb;

        private void Start()
        {
            cam = Camera.main;
            lr = GetComponent<LineRenderer>();

            playerCharacter = player.GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && playerCharacter.moveable)
            {
                Vector2 dragPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            
                Vector2 direction = (startPoint - dragPoint).normalized;
                float magnitude = (startPoint - dragPoint).magnitude;
            
                Vector2 velocity =  power * Math.Min(magnitude, _selectedProjectile.getMaxMagnitude()) * direction;

            
                Vector2[] trajectory = Plot(_rb, (Vector2)transform.position, velocity, _selectedProjectile.getSteps());
                lr.positionCount = trajectory.Length;

                Vector3[] positions = new Vector3[trajectory.Length];
                for (int i = 0; i < trajectory.Length; i++)
                {
                    positions[i] = trajectory[i];
                }
                lr.SetPositions(positions);
            }
            else
            {
                lr.enabled = false;
            }
        }

        private void OnMouseDown()
        {
            if (playerCharacter.moveable)
            {
                startPoint = cam.ScreenToWorldPoint(Input.mousePosition);
                lr.enabled = true;
            }
        }

        private void OnMouseUp()
        {
            if (playerCharacter.moveable)
            {

                endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        
                Vector2 direction = (startPoint - endPoint).normalized;
                float magnitude = (startPoint - endPoint).magnitude;
        
                Vector2 velocity = power * Math.Min(magnitude, _selectedProjectile.getMaxMagnitude()) * direction;

                GameObject projectile = Instantiate(_projectilePrefab, gameObject.transform.position, transform.rotation);
                Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
                prb.velocity = velocity;

                gameController.projectileShot = true;
                playerCharacter.moveable = false;
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

        public void RefreshPrefab()
        {
            _projectilePrefab = WeaponManager.Instance.GetWeaponById(selectedWeaponId).projectilePrefab;
            _rb = _projectilePrefab.GetComponent<Rigidbody2D>();
            _selectedProjectile = _projectilePrefab.GetComponent<IProjectile>();
        }
    }
}
