using UnityEngine;
#if  UNITY_EDITOR
using UnityEditor;
#endif

namespace TerraformingTerrain2d
{
    [ExecuteInEditMode]
    public class TerraformingTerrain2D : MonoBehaviour
    {
        [SerializeField] private TerraformingTerrainData _data;
        private TerraformingTerrainFacade _facade;
        private bool _onValidate;
        
        public TerraformingTerrainData Data => _data;

        private void OnValidate()
        {
            _onValidate = true;
        }

        private void Start()
        {
            Reassemble();
        }

        #if  UNITY_EDITOR
        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += Dispose;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= Dispose;
        }
        #endif

        private void Reassemble()
        {
            _onValidate = false;
            
            if (_data != null && _data.Input.CheckIfValid())
            {
                _data.OnValidate();
                _data.PassTransform(transform);
                
                Dispose();
                _facade = new TerraformingTerrainFacade(_data);
                _facade.ReassembleChunks();

                if (_data.RequiredToRegenerate)
                {
                    Regenerate();
                }
            }
        }

        public void Terraform(Vector2 position, float radius, TerraformingMode mode)
        {
            _facade.Terraform(position, radius, mode);
        }

        public void Clear()
        {
            _facade.Clear();
        }
        
        private void Regenerate()
        {
            _data.SaveGeneratedChunksCount();
            _facade.Regenerate();
        }

        private void Update()
        {
            if (_onValidate)
            {
                Reassemble();
            }

            _facade?.UpdateTransform();
        }

        private void Dispose()
        {
            _facade?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}