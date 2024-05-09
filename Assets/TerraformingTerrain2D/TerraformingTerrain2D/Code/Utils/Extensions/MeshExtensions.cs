using UnityEngine;

namespace TerraformingTerrain2d
{
    public static class MeshExtensions
    {
        public static Mesh BuildQuad(float size)
        {
            return BuildQuad(size, size);
        }

        public static Mesh BuildQuad(float width, float height)
        {
            Mesh mesh = new();

            Vector3[] vertices = new Vector3[4]
            {
                new(0 - width / 2, -height / 2, 0),
                new(width - width / 2, -height / 2, 0),
                new(0 - width / 2, height - height / 2, 0),
                new(width - width / 2, height - height / 2, 0)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new(0, 0),
                new(1, 0),
                new(0, 1),
                new(1, 1)
            };
            mesh.uv = uv;

            return mesh;
        }
    }
}