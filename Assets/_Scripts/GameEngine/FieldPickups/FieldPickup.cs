using UnityEngine;

namespace _Scripts.GameEngine.FieldPickups
{
    public abstract class FieldPickup : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            Trigger();
            Destroy(gameObject);
        }

        protected abstract void Trigger();
    }
}