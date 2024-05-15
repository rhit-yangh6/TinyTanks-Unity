using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Buffs;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Managers
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

        public enum DamageType
        {
            Circular,
            Square
        }
        
        public int HandleDamage(
            Vector2 pos, 
            float radius, 
            float damage, 
            DamageType type,
            bool isCriticalHit = false, 
            ScriptableBuff buff = null, 
            int buffLevel = 1)
        {
            var hitColliders = type switch
            {
                DamageType.Circular => Physics2D.OverlapCircleAll(pos, radius, layerMask),
                // TODO: Calculating the square damage
                DamageType.Square => Physics2D.OverlapBoxAll(pos, new Vector2(radius*1.414f, radius*1.414f), 
                    0,layerMask),
                _ => null
            };

            var hitCount = 0;

            foreach(var col in hitColliders)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb == null) continue;

                hitCount += 1;

                // Find the Enemy script and apply damage.
                var e = rb.gameObject.GetComponent<Entity>();
                var roundedDamage = (float)Math.Round(damage);
                e.TakeDamage(roundedDamage, isCriticalHit);

                var be = rb.gameObject.GetComponent<BuffableEntity>();
                // Apply Buff
                if (be != null && buff != null)
                {
                    be.AddBuff(buff.InitializeBuff(col.gameObject, buffLevel));
                }
                
                /*
                // TODO: Push Force
                if (force != 0f)
                {
                    var forceDirection = Vector3.Normalize(rb.position - pos);
                    rb.AddForce(forceDirection * force);
                }
                */
            }

            return hitCount;
        }

        public Entity DetectNearestTarget(Vector2 pos, float radius, Entity excludingEntity)
        {
            var hitColliders = Physics2D.OverlapCircleAll(pos, radius, layerMask);
            var entities =
                hitColliders
                    .Select(col => col.gameObject.GetComponent<Entity>())
                    .Where(e => e != null).ToList();

            Entity closetEntity = null;
            var minDist = Mathf.Infinity;
            foreach(var e in entities)
            {
                if (ReferenceEquals(e, excludingEntity)) continue;
                var distance = Vector2.Distance(pos, e.transform.position);
                if (distance >= minDist) continue;
                closetEntity = e;
                minDist = distance;
            }

            return closetEntity;
        }

        public IEnumerable<Entity> DetectTargets(Vector2 pos, float radius)
        {
            var hitColliders = Physics2D.OverlapCircleAll(pos, radius, layerMask);

            return hitColliders.Select(col => col.gameObject.GetComponent<Entity>()).Where(e => e != null);
        }

        public void HandleCompleteHeals(Vector2 pos, float radius, ScriptableBuff buff = null)
        {
            var hitColliders = Physics2D.OverlapCircleAll(pos, radius, layerMask);

            foreach (var col in hitColliders)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb == null) continue;
                
                var e = rb.gameObject.GetComponent<Entity>();
                e.CompleteHeal();
                
                var be = rb.gameObject.GetComponent<BuffableEntity>();
                // Apply Buff
                if (be != null && buff != null)
                {
                    be.AddBuff(buff.InitializeBuff(col.gameObject, 1));
                }
            }
        }

    }
}
