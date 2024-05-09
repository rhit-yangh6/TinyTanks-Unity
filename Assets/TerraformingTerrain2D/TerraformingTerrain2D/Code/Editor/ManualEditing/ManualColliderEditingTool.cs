using TerraformingTerrain2d;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace TerraformingTerrain2dEditor
{
    [EditorTool("Manual collider editing", typeof(TerraformingTerrain2D))]
    public class ManualColliderEditingTool : EditorTool
    {
        private EditorTerraforming _editorTerraforming;
        private Arc _arc;

        public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("d_MeshCollider Icon");

        private void OnEnable()
        {
            _arc = new Arc();
            _editorTerraforming = new EditorTerraforming();
        }

        [Shortcut("Manual collider editing tool", KeyCode.U)]
        private static void PlatformToolShortcut()
        {
            if (Selection.GetFiltered<TerraformingTerrain2D>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<ManualColliderEditingTool>();
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (window is SceneView)
            {
                TerraformingTerrain2D terrain2D = (TerraformingTerrain2D)target;

                _arc.Draw();
                _editorTerraforming.DefineUserState();

                if (Event.current.control)
                {
                    _arc.TryUpdateRadius();
                    _editorTerraforming.TryTerraform(terrain2D, _arc.Radius * 2);
                }

                AddDefaultControl();
            }
        }

        private void AddDefaultControl()
        {
            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
        }
    }
}