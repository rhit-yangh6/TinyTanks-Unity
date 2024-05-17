using UnityEngine;

namespace _Scripts.Utils
{
    public static class Geometry
    {
        public static Vector2 Rotate(Vector2 v, float delta)
        {
            var deltaRad = delta * Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(deltaRad) - v.y * Mathf.Sin(deltaRad),
                v.x * Mathf.Sin(deltaRad) + v.y * Mathf.Cos(deltaRad)
            );
        }
    }
}