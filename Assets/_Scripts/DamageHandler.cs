using System;
using UnityEngine;

namespace _Scripts
{
    public class DamageHandler : MonoBehaviour
    {
        public static DamageHandler i { get; private set; }
        
        public LayerMask layerMask;
        
        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
            if (i != null && i != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                i = this; 
            } 
        }
        
        public void HandleCircularDamage(Vector2 pos, float radius, float damage, bool isCriticalHit = false, float force = 0f)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos, radius, layerMask);
            
            foreach(Collider2D col in hitColliders)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb == null) continue;
                
                // Popup Damage
                DamagePopup.Create(rb.position, (int)Math.Round(damage), isCriticalHit);
                
                // Find the Enemy script and apply damage.
                var c = rb.gameObject.GetComponent<Character>();
                c.TakeDamage((float)Math.Round(damage));
                
                /*
                // Push Force
                if (force != 0f)
                {
                    var forceDirection = Vector3.Normalize(rb.position - pos);
                    rb.AddForce(forceDirection * force);
                }
                */
                // TODO: Props?
            }
        }

    }
}
