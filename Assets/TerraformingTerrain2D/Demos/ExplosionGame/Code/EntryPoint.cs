using UnityEngine;

namespace ExplosionGame
{
    [DefaultExecutionOrder(-10)]
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private ServicesRunner _servicesRunner;
        [SerializeField] private InputProvider _inputProvider;
        [SerializeField] private Weapon _initialWeapon;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private Unit _unit;

        private void Awake()
        {
            IInput input = _inputProvider.GetInput();
            MovementState movementState = new(input);
            DieState dieState = new(new EmptyAnimation());

            _inventory.Compose(_unit, _initialWeapon);
            _servicesRunner.Compose(dieState);
            _unit.Compose(movementState, _initialWeapon, dieState);
        }
    }
}