using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class MolotovProjectile : LaunchedProjectile
    {
        [SerializeField] private GameObject flamePrefab;
        [SerializeField] private float flameRadius = 0.23f;
        [SerializeField] private float flameSpeed = 3.5f;
        
        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    >= 2 => damage * 1.2f,
                    _ => damage
                };
            }
        }
        
        protected override float MaxMagnitude
        {
            get
            {
                return Level switch
                {
                    >= 2 => maxMagnitude * 1.1f,
                    _ => maxMagnitude
                };
            }
        }

        private int FlameNumber
        {
            get
            {
                return Level switch
                {
                    6 => 20,
                    >= 3 => 13,
                    _ => 10,
                };
            }
        }

        private float FlameDamage
        {
            get
            {
                return Level switch
                {
                    >= 4 => 3,
                    _ => 2
                };
            }
        }
        
        private float FlameLifeSpan
        {
            get
            {
                return Level switch
                { 
                    5 => 3.5f,
                    _ => 2f
                };
            }
        }

        private float FlameSpreadRange
        {
            get
            {
                return Level switch
                {
                    6 => 160f,
                    >= 4 => 140f,
                    _ => 130f,
                };
            }
        }
        
        private void Update()
        {
            Spin(1.6f);
        }
        
        public override void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
        }

        public void SpawnFlames()
        {
            Vector2 pos = transform.position;
            for (var i = 0; i < FlameNumber; i++)
            {
                var derivedObject = Instantiate(flamePrefab, pos + Vector2.up, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<FlameProjectile>();
                var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
                derivedProjectile.SetParameters(FlameDamage, flameRadius);
                derivedProjectile.SetLifeSpan(FlameLifeSpan);

                var angle = Random.Range(-FlameSpreadRange / 2, FlameSpreadRange / 2);
                derivedRb2d.velocity =  Geometry.Rotate(Vector2.up * flameSpeed, angle);
            }
        }
    }
}