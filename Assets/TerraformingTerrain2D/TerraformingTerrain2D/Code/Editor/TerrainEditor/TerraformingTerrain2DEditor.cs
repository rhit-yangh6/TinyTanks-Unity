using EditorWrapper;
using TerraformingTerrain2d;
using UnityEditor;

namespace TerraformingTerrain2dEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TerraformingTerrain2D))]
    public class TerraformingTerrain2DEditor : Editor
    {
        private IDrawable _editor;

        private void OnEnable()
        {
            _editor = new TerrainEditorFactory(serializedObject, (TerraformingTerrain2D)target).Create();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _editor.Draw();

            serializedObject.ApplyModifiedProperties();
        }
    }
}