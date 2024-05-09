using TerraformingTerrain2d;
using UnityEngine;

namespace ExplosionGame
{
    public class Uzi : Weapon
    {
        [SerializeField] private float _rate = 0.3f;
        [SerializeField] private float _carveRadius = 0.2f;
        [SerializeField] private Explosion _explosion;
        private float _timer;

        public override void Shoot()
        {
            _timer += Time.deltaTime;

            if (_timer > _rate)
            {
                _timer = 0;

                ShootTerrain();
            }
        }

        private void ShootTerrain()
        {
            _explosion.Play();

            if (TryShootChunk(out TerraformingTerrain2dChunk chunk, out Vector2 hitPoint))
            {
                chunk.TerraformingPresenter.Rebuild(hitPoint, _carveRadius, TerraformingMode.Carve);
            }
        }

        private bool TryShootChunk(out TerraformingTerrain2dChunk chunk, out Vector2 hitPoint)
        {
            int randomSign = Random.Range(0, 2) == 0 ? -1 : 1;
            Vector3 noise = SpawnPoint.up * (Random.Range(0, 0.025f) * randomSign);
            Vector3 direction = SpawnPoint.right + noise;
            chunk = null;
            hitPoint = Vector2.zero;
            RaycastHit2D hit = Physics2D.Raycast(SpawnPoint.position, direction);

            if (hit.collider.TryGetComponent(out chunk))
            {
                hitPoint = hit.point;
            }

            return chunk != null;
        }
    }
}