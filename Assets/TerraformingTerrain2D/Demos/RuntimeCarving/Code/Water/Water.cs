using UnityEngine;

namespace RuntimeCarving
{
    public class Water : MonoBehaviour
    {
        [SerializeField] private WaterParticle _prefab;
        [SerializeField] private Vector2Int _count;
        [SerializeField] private float _offset = 0.4f;

        [ContextMenu("Regenerate")]
        private void Regenerate()
        {
            WaterParticle[] waterParticles = GetComponentsInChildren<WaterParticle>(true);

            for (int i = 0; i < waterParticles.Length; ++i)
            {
                DestroyImmediate(waterParticles[i].gameObject);
            }

            for (int x = 0; x < _count.x; ++x)
            {
                for (int y = 0; y < _count.y; ++y)
                {
                    WaterParticle waterParticle = Instantiate(_prefab, transform);
                    waterParticle.transform.localPosition = new Vector3(x, y, 0) * _offset;
                    waterParticle.gameObject.layer = gameObject.layer;
                }
            }
        }
    }
}