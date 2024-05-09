using UnityEditor;
using UnityEngine;

namespace TerraformingTerrain2dEditor
{
    public class Arc
    {
        public float Radius { get; private set; } = 0.25f;
        private readonly float _speed = 0.01f;

        public void TryUpdateRadius()
        {
            Event guiEvent = Event.current;

            if (guiEvent.isScrollWheel)
            {
                guiEvent.Use();
                Radius -= guiEvent.delta.y * _speed;
                Radius = Mathf.Max(Radius, 0);
            }
        }

        public void Draw()
        {
            Handles.color = new Color(0, 1, 0, 0.4f);
            Handles.DrawSolidArc(EditorUtils.GetSceneWorldMousePosition(), Vector3.forward, Vector3.up, 360, Radius);
        }
    }
}