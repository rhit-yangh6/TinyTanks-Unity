using UnityEngine;

namespace TerraformingTerrain2d
{
    public class TransformUpdate
    {
        private readonly ChunksPresenter _chunksPresenter;
        private readonly Transform _transform;

        public TransformUpdate(ChunksPresenter chunksPresenter, Transform transform)
        {
            _chunksPresenter = chunksPresenter;
            _transform = transform;
        }

        public void Update()
        {
            if (_transform.hasChanged)
            {
                _chunksPresenter.UpdateColliderOffset();

                if (_transform.parent != null)
                {
                    Debug.LogError("Terrain cannot have parent transform");
                    _transform.parent = null;
                }

                if (_transform.localScale != Vector3.one)
                {
                    Debug.LogError("Terrain transform scale must be (1,1,1). Use \"Scale\" property in \"View\" section instead");
                    _transform.localScale = Vector3.one;
                }
            }
        }
    }
}