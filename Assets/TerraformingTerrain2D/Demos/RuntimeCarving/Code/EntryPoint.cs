using DemosShared;
using UnityEngine;

namespace RuntimeCarving
{
    [DefaultExecutionOrder(-10)]
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private ServicesRunner _servicesRunner;
        [SerializeField] private Star[] _stars;

        private void Awake()
        {
            foreach (Star star in _stars)
            {
                star.Compose();
            }

            _servicesRunner.Compose(_stars);
        }
    }
}