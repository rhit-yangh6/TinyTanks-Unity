using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    public class ChargeableWeapon : Weapon, ICoroutineRunner
    {
        [SerializeField] private WeaponChargeView _weaponChargeView;
        [SerializeField] private Projectile _projectile;
        private MonoBehaviourFactory<Projectile> _factory;
        private WeaponCharge _weaponCharge;

        private void Start()
        {
            _weaponCharge = new WeaponCharge(this);
            _factory = new MonoBehaviourFactory<Projectile>(_projectile);
        }

        public override void Shoot()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _weaponCharge.Charge(OnEnergyCharging, OnEnergyCharged);
            }
        }

        private void OnEnergyCharged(float accumulatedEnergy)
        {
            float angle = Mathf.Atan2(SpawnPoint.right.y, SpawnPoint.right.x);

            Projectile projectile = _factory.Create();
            projectile.Shoot(SpawnPoint, angle, accumulatedEnergy);

            _weaponChargeView.Hide();
        }

        private void OnEnergyCharging(float lerp)
        {
            _weaponChargeView.SetCharge(lerp);
        }
    }
}