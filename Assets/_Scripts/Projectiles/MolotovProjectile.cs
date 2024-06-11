using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class MolotovProjectile : LaunchedProjectile
    {
        [SerializeField] private int flameNumber = 10;
        [SerializeField] private GameObject flamePrefab;
        [SerializeField] private float flameLifeSpan = 2.5f;
        [SerializeField] private float flameDamage = 3f;
        [SerializeField] private float flameRadius = 0.23f;
        [SerializeField] private float flameSpreadRange = 160f;
        [SerializeField] private float flameSpeed = 4f;
        
        private void Update()
        {
            Spin();
        }
        
        public override void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
        }

        public void SpawnFlames()
        {
            Vector2 pos = transform.position;
            for (var i = 0; i < flameNumber; i++)
            {
                var derivedObject = Instantiate(flamePrefab, pos + Vector2.up, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<FlameProjectile>();
                var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
                derivedProjectile.SetParameters(flameDamage, flameRadius);
                derivedProjectile.SetLifeSpan(flameLifeSpan);

                var angle = Random.Range(-flameSpreadRange / 2, flameSpreadRange / 2);
                derivedRb2d.velocity =  Geometry.Rotate(Vector2.up * flameSpeed, angle);
            }
        }
    }
}