using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    public class Unit : MonoBehaviourWrapper
    {
        [SerializeField] private UnitViewComponents _components;
        [SerializeField] private PhysicsOverlapCheck _overlapCheck;
        [SerializeField] private float _speed = 5;
        public WeaponHolder WeaponHolder { get; private set; }

        public void Compose(MovementState movementState, Weapon defaultWeapon, DieState dieState)
        {
            UnitStateMachineFactory stateMachineFactory = new(_components, movementState, _overlapCheck, dieState, _speed);
            StateMachine unitStateMachine = stateMachineFactory.Create();
            WeaponHolder = new WeaponHolder(defaultWeapon);
            defaultWeapon.Equip();

            SetDependencies(new IUnityCallback[]
            {
                new SpriteFlipping(WeaponHolder, movementState, _components.SpriteRenderer),
                new WeaponShooting(WeaponHolder),
                unitStateMachine,
            });
        }
    }
}