using UnityEngine;

namespace DemosShared
{
    public class MonoBehaviourFactory<T> where T : MonoBehaviour
    {
        private readonly T _prefab;

        public MonoBehaviourFactory(T prefab)
        {
            _prefab = prefab;
        }

        public T Create()
        {
            T result = Object.Instantiate(_prefab);
            result.transform.position = Vector3.one * 1000;

            return result;
        }
    }
}