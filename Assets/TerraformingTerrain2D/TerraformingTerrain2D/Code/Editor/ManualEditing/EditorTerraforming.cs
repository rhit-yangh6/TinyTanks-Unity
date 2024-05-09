using TerraformingTerrain2d;
using UnityEngine;

namespace TerraformingTerrain2dEditor
{
    public class EditorTerraforming
    {
        private bool _isCarving;
        private bool _isFilling;

        public void DefineUserState()
        {
            Event guiEvent = Event.current;

            if (guiEvent.type == EventType.MouseDown)
            {
                if (guiEvent.control)
                {
                    if (guiEvent.button == 0)
                    {
                        _isCarving = true;
                    }
                    else if (guiEvent.button == 1)
                    {
                        _isFilling = true;
                    }
                }
            }

            if (guiEvent.type == EventType.MouseUp)
            {
                _isCarving = false;
                _isFilling = false;
            }
        }

        public void TryTerraform(TerraformingTerrain2D terrain, float radius)
        {
            if (_isCarving || _isFilling)
            {
                TerraformingMode mode = _isCarving ? TerraformingMode.Carve : TerraformingMode.Fill;
                terrain.Terraform(EditorUtils.GetSceneWorldMousePosition(), radius, mode);
            }
        }
    }
}