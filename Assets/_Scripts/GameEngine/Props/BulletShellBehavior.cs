using System.Collections;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.GameEngine.Props
{
    public class BulletShellBehavior : MonoBehaviour
    {
        [SerializeField] private float initialVelocity;
        [SerializeField] private float lifeTime;
        [SerializeField] private float dispersionAngle;

        private void Start()
        {
            var rigidBody2D = GetComponent<Rigidbody2D>();
            rigidBody2D.linearVelocity = Geometry.Rotate(Vector2.up, 
                Random.Range(-dispersionAngle, dispersionAngle)) * initialVelocity;
            rigidBody2D.rotation = Random.Range(-360, 360);
            StartCoroutine(DisappearAfterLifetime());
        }

        private IEnumerator DisappearAfterLifetime()
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(gameObject);
        }
    }
}