using UnityEngine;

namespace DemosShared
{
    public class PhysicsOverlapCheck : MonoBehaviour
    {
        [SerializeField] [Min(0)] private float _radius = 0.2f;

        public bool CheckCollision<T>() where T : MonoBehaviour
        {
            return GetCollidedObject<T>() != null;
        }
        
        public T GetCollidedObject<T>() where T : MonoBehaviour
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _radius);

            for (int i = 0; i < colliders.Length; ++i)
            {
                T hitObject = colliders[i].GetComponent<T>();
                
                if (hitObject != null)
                    return hitObject;
            }

            return null;
        }

        public bool HasCollision(LayerMask mask) => CheckCollision(mask);

        private bool CheckCollision(LayerMask mask) => Physics2D.OverlapCircle(transform.position, _radius, mask) != null;

        private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position, _radius);
    }
}