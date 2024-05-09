using TerraformingTerrain2d;
using UnityEditor;
using UnityEngine;

namespace SDFGeneration
{
    #if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SDFTextureFromSpriteShapeGenerator))]
    public class SpriteShapeSDFTextureGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Create SDF texture", GUILayout.Width(150), GUILayout.Height(25)))
            {
                SDFTextureFromSpriteShapeGenerator sdfTextureFromSpriteShapeGenerator = (SDFTextureFromSpriteShapeGenerator)target;
                RenderTexture texture = sdfTextureFromSpriteShapeGenerator.Generate();
                FileSaveUtils.SaveTexture(texture.SaveRenderTextureAsPng());
            }
        }
    }
    #endif
}
