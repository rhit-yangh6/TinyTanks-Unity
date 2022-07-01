using UnityEngine;

// Source: https://github.com/Bardent/Rigidbody2D-Slopes-Unity/blob/master/Assets/Scripts/PlayerController.cs
namespace _Scripts
{
    public class PlayerController : MonoBehaviour, Character
    {
        public float Health { get; set; }

        public float maxHealth = 100;

        public bool moveable;

        public float movementSpeed = 7;
        public LayerMask layerMask;
        public PlayerHealthBarBehavior healthBar;


        private float xInput;
        private int facingDirection = 1;
        private SpriteRenderer sr;

        private void Start()
        {
            Health = maxHealth;
            
            healthBar.SetHealth(Health, maxHealth);

            sr = GetComponent<SpriteRenderer>();
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
                xInput = Input.GetAxisRaw("Horizontal");

                if (xInput == 1 && facingDirection == -1)
                {
                    Flip();
                }
                else if (xInput == -1 && facingDirection == 1)
                {
                    Flip();
                } 

                transform.Translate(Time.deltaTime * movementSpeed * new Vector3(xInput, 0, 0));
                
            }
        }
    
        public void Flip()
        {
            facingDirection *= -1;
            sr.flipX = facingDirection == -1;
        }
    }
}
