using UnityEditor;
using UnityEngine;

namespace SDFGeneration
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SDFTextureFromImageGenerator))]
    public class SDFTextureFromImageGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Create destructible terrain"))
            {
                SDFTextureFromImageGenerator setup = (SDFTextureFromImageGenerator)target;
                setup.Setup();
            }
        }
    }
#endif
}