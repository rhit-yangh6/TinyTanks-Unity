using System.Collections;
using TerraformingTerrain2d;
using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    public class PhysicsProjectile : Projectile
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ClusterGrenadeUI _ui;
        [SerializeField] private int _timeForExplosion;
        [SerializeField] private PhysicsOverlapCheck _overlapCheck;
        
        public override void Shoot(Transform spawnPoint, float angle, float energy)
        {
            transform.position = spawnPoint.position;
            _rigidbody2D.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

            _rigidbody2D.AddForce(spawnPoint.right * (energy * 0.9f), ForceMode2D.Impulse);
            _rigidbody2D.AddTorque(-energy / 2f, ForceMode2D.Force);
            
            Destroy(gameObject, _timeForExplosion + 0.5f);
            Destroy(_ui.gameObject, _timeForExplosion + 0.5f);
            
            _ui.Unpin();
            StartCoroutine(PinTimeUI());
            StartCoroutine(StartCountdown());
        }

        private IEnumerator PinTimeUI()
        {
            while (true)
            {
                _ui?.Move(transform.position);
                yield return null;
            }
        }

        private IEnumerator StartCountdown()
        {
            int currentTime = _timeForExplosion;

            for (int i = 0; i < _timeForExplosion; ++i, --currentTime)
            {
                _ui.UpdateValue(currentTime);
                yield return new WaitForSeconds(1f);
            }

            TerraformingTerrain2dChunk chunk = _overlapCheck.GetCollidedObject<TerraformingTerrain2dChunk>();

            if (chunk != null)
            {
                Destroy(_ui.gameObject);
                Explode(chunk);
            }
        }
    }
}