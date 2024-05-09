using System;
using System.Collections;
using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    public class WeaponCharge
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly float _chargingSpeed = 10f;
        private readonly float _maxEnergy = 10f;
        private float _accumulatedEnergy;

        public WeaponCharge(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void Charge(Action<float> onCharging, Action<float> onCharged)
        {
            _coroutineRunner.StartCoroutine(StartCharging(onCharging, onCharged));
        }

        private IEnumerator StartCharging(Action<float> onCharging, Action<float> onCharged)
        {
            yield return ChargeEnergy(onCharging);
            onCharged(_accumulatedEnergy);
        }

        private IEnumerator ChargeEnergy(Action<float> onCharging)
        {
            _accumulatedEnergy = 0;

            while (IsStopCondition())
            {
                _accumulatedEnergy += Time.deltaTime * _chargingSpeed;
                float lerp = Mathf.Min(1, _accumulatedEnergy / _maxEnergy);
                onCharging(lerp);

                yield return null;
            }
        }

        private bool IsStopCondition()
        {
            return _accumulatedEnergy < _maxEnergy &&
                   Input.GetMouseButton(0);

        }
    }
}