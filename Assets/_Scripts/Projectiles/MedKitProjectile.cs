using UnityEngine;

namespace _Scripts.Projectiles
{
    public class MedKitProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        // private static float _boulderPieceRadius, _boulderPieceDamage;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        public override void Detonate()
        {
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleCircularDamage(pos, Radius, Damage);
            
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;

            // _boulderPieceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceDamage").value;
            // _boulderPieceRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceRadius").value;
        }
    }
}