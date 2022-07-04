using UnityEngine;

// Source: https://github.com/Bardent/Rigidbody2D-Slopes-Unity/blob/master/Assets/Scripts/PlayerController.cs
namespace _Scripts
{
    public class PlayerController : MonoBehaviour, Character
    {
        public float Health { get; set; }

        public float maxHealth = 100;

        public bool moveable;
        
        [HideInInspector] public int facingDirection = 1;
        
        public float movementSpeed = 7;
        public LayerMask layerMask;
        public PlayerHealthBarBehavior healthBar;
        public GameObject tankCannon;
        
        private float _xInput;
        private SpriteRenderer _mainSr, _cannonSr;

        private void Start()
        {
            Health = maxHealth;
            
            healthBar.SetHealth(Health, maxHealth);

            _mainSr = GetComponent<SpriteRenderer>();
            _cannonSr = tankCannon.GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
            CheckMovement();
            AdjustRotation();
        }
    
        public void TakeDamage(float amount)
        {
            if (Health - amount < 0)
            {
                Health = 0;
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
                /*
            Debug.Log(Vector2.SignedAngle(hit.normal, Vector2.up));
            // Draw lines to show the incoming "beam" and the reflection.
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
            */
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
            if (moveable)
            {
                _xInput = Input.GetAxisRaw("Horizontal");

                if (_xInput == 1 && facingDirection == -1)
                {
                    Flip();
                }
                else if (_xInput == -1 && facingDirection == 1)
                {
                    Flip();
                } 

                transform.Translate(Time.deltaTime * movementSpeed * new Vector3(_xInput, 0, 0));
                
            }
        }
    
        public void Flip()
        {
            facingDirection *= -1;
            _mainSr.flipX = facingDirection == -1;
            _cannonSr.flipX = facingDirection == -1;
        }

        public void SetCannonAngle(float angle)
        {
            tankCannon.transform.localEulerAngles = (facingDirection == 1 ? -angle : (180 - angle)) * Vector3.forward;
        }
    }
}
