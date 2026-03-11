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
            var hitColliders = OverlapByType(pos, radius, type);
            var hitCount = 0;

            foreach (var col in hitColliders)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb == null) continue;

                var e = rb.gameObject.GetComponent<Entity>();
                if (e == null) continue;

                hitCount += 1;

                var roundedDamage = (float)Math.Round(damage);
                e.TakeDamage(roundedDamage, isCriticalHit);

                ApplyBuff(col.gameObject, buff, buffLevel);
            }
            return hitCount;
        }

        public int HandleDamageExcludingEntity(
            Vector2 pos,
            float radius,
            float damage,
            DamageType type,
            Entity excludingEntity,
            bool isCriticalHit = false,
            ScriptableBuff buff = null,
            int buffLevel = 1)
        {
            var hitColliders = OverlapByType(pos, radius, type);
            var hitCount = 0;

            foreach (var col in hitColliders)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb == null) continue;

                var e = rb.gameObject.GetComponent<Entity>();
                if (e == null || ReferenceEquals(e, excludingEntity)) continue;

                var roundedDamage = (float)Math.Round(damage);
                e.TakeDamage(roundedDamage, isCriticalHit);

                hitCount += 1;

                ApplyBuff(col.gameObject, buff, buffLevel);
            }
            return hitCount;
        }

        public Entity DetectNearestTarget(Vector2 pos, float radius, List<Entity> excludingEntities)
        {
            var hitColliders = Physics2D.OverlapCircleAll(pos, radius, layerMask);
            var excludeSet = new HashSet<Entity>(excludingEntities);

            Entity closestEntity = null;
            var minDist = Mathf.Infinity;

            foreach (var col in hitColliders)
            {
                var e = col.gameObject.GetComponent<Entity>();
                if (e == null || excludeSet.Contains(e)) continue;

                var distance = Vector2.Distance(pos, e.transform.position);
                if (distance >= minDist) continue;
                closestEntity = e;
                minDist = distance;
            }

            return closestEntity;
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
                if (e == null) continue;

                e.CompleteHeal();

                ApplyBuff(col.gameObject, buff, 1);
            }
        }

        private static void ApplyBuff(GameObject obj, ScriptableBuff buff, int buffLevel)
        {
            if (buff == null) return;
            var be = obj.GetComponent<BuffableEntity>();
            if (be != null)
            {
                be.AddBuff(buff.InitializeBuff(obj, buffLevel));
            }
        }

        private Collider2D[] OverlapByType(Vector2 pos, float radius, DamageType type)
        {
            return type switch
            {
                DamageType.Circular => Physics2D.OverlapCircleAll(pos, radius, layerMask),
                DamageType.Square => Physics2D.OverlapBoxAll(pos,
                    new Vector2(radius * 1.414f, radius * 1.414f), 0, layerMask),
                _ => Array.Empty<Collider2D>()
            };
        }
    }
}
