using UnityEditor;
using UnityEngine;

namespace TerraformingTerrain2dEditor
{
    public static class EditorUtils
    {
        public static Vector3 GetSceneWorldMousePosition()
        {
            Vector3 worldPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            worldPosition.z = 0;

            return worldPosition;
        }
    }
}