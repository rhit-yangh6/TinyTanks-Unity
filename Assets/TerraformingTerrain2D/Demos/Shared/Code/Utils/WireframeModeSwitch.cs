using UnityEngine;

namespace DemosShared
{
    public class WireframeModeSwitch : MonoBehaviour
    {
        private bool _isWireframe;

        public void EnableWireframeMode()
        {
            _isWireframe = true;
        }

        public void EnableShadedMode()
        {
            _isWireframe = false;
        }

        public void OnPreRender()
        {
            if (_isWireframe)
            {
                GL.wireframe = true;
            }
        }

        public void OnPostRender()
        {
            if (_isWireframe)
            {
                GL.wireframe = false;
            }
        }
    }
}