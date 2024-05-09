using UnityEngine;

namespace TerraformingTerrain2d
{
    public class Rectangle
    {
        private readonly Transform _transform;
        private readonly Rect _rect;

        public Rectangle(Rect rect, Transform transform)
        {
            _transform = transform;
            _rect = rect;
        }

        private float RotationAngle => _transform.rotation.eulerAngles.z;
        private Vector2 RectangleCentre => _transform.TransformPoint(_rect.center);

        public void DrawGizmo()
        {
            Vector3 position = _transform.position - _transform.forward;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(0, 0, RotationAngle), Vector3.one);

            Color color = Gizmos.color;
            
            Gizmos.color = new Color(1f, 0.25f, 0.15f, 0.25f);
            Gizmos.DrawCube(new Vector3(_rect.center.x, _rect.center.y, 0.01f), new Vector3(_rect.size.x, _rect.size.y, 0.01f));
            
            Gizmos.color = new Color(1f, 0.25f, 0.15f, 1);
            Gizmos.DrawWireCube(new Vector3(_rect.center.x, _rect.center.y, 0.01f), new Vector3(_rect.size.x, _rect.size.y, 0.01f));
            
            Gizmos.color = color;
        }

        public bool IntersectsCircle(Vector2 circleCentre, float radius)
        {
            Vector2 rotatedCircleCentre = Quaternion.Euler(0, 0, -RotationAngle) * (circleCentre - RectangleCentre);

            float closestX = Mathf.Clamp(rotatedCircleCentre.x, -_rect.width / 2, _rect.width / 2);
            float closestY = Mathf.Clamp(rotatedCircleCentre.y, -_rect.height / 2, _rect.height / 2);

            Vector2 closestPoint = new(closestX, closestY);

            Vector2 distanceVector = rotatedCircleCentre - closestPoint;
            float distanceSquared = distanceVector.sqrMagnitude;

            return distanceSquared < (radius * radius);
        }
    }
}