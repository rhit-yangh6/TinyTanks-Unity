
namespace ExplosionGame
{
    public class WeaponHolder
    {
        private Weapon _weapon;

        public WeaponHolder(Weapon defaultWeapon)
        {
            _weapon = defaultWeapon;
        }

        public Weapon Current => _weapon;

        public void SetWeapon(Weapon weapon)
        {
            _weapon.Hide();
            _weapon = weapon;
            _weapon.Equip();
        }

        public void Shoot()
        {
            _weapon.Shoot();
        }
    }
}