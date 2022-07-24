using System;
using _Scripts.Buffs;
using _Scripts.Entities;
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
        
        public void HandleCircularDamage(Vector2 pos, float radius, float damage,
            bool isCriticalHit = false, ScriptableBuff buff = null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos, radius, layerMask);
            
            foreach(Collider2D col in hitColliders)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb == null) continue;
                
                // Popup Damage
                DamagePopup.Create(rb.position, (int)Math.Round(damage), isCriticalHit);
                
                // Find the Enemy script and apply damage.
                var e = rb.gameObject.GetComponent<Entity>();
                e.TakeDamage((float)Math.Round(damage));

                var be = rb.gameObject.GetComponent<BuffableEntity>();
                // Apply Buff
                if (be != null && buff != null)
                {
                    be.AddBuff(buff.InitializeBuff(col.gameObject));
                }
                
                /*
                // TODO: Push Force
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
