using TerraformingTerrain2d;
using UnityEngine;
using UnityEngine.U2D;

namespace SDFGeneration
{
#if UNITY_EDITOR
    [RequireComponent(typeof(PolygonCollider2D), typeof(SpriteShapeController))]
    public class SDFTextureFromSpriteShapeGenerator : MonoBehaviour
    {
        [SerializeField] private int _size = 500;
        private readonly float _sdfFactor = 1.21f;

        public RenderTexture Generate()
        {
            Vector2[] points = GetPoints();
            MinMax minMax = GetMinMax(points);

            ComputeBuffer computeBuffer = new(points.Length, 8);
            computeBuffer.SetData(points);

            RenderTexture renderTexture = CreateRenderTexture(minMax);
            Material blitMaterial = CreateBlitMaterial(computeBuffer, points.Length, minMax.Value);

            Graphics.Blit(null, renderTexture, blitMaterial, 0);

            computeBuffer.Release();

            return renderTexture;
        }

        private RenderTexture CreateRenderTexture(MinMax minMax)
        {
            float ratio = (minMax.Max - minMax.Min).y / (minMax.Max - minMax.Min).x;
            RenderTexture renderTexture = new(_size, (int)(_size * ratio), 0, RenderTextureFormat.ARGB32);

            return renderTexture;
        }

        private Material CreateBlitMaterial(ComputeBuffer computeBuffer, int verticesCount, Vector4 minMax)
        {
            Material material = new(Shader.Find("Unlit/PolygonSDF"));
            material.SetBuffer("_Vertices", computeBuffer);
            material.SetInt("_VerticesCount", verticesCount);
            material.SetVector("_MinMax", minMax);
            material.SetFloat("_SdfFactor", _sdfFactor);

            return material;
        }

        private Vector2[] GetPoints()
        {
            PolygonCollider2D polygonCollider2D = GetComponent<PolygonCollider2D>();

            Vector2[] points = new Vector2[polygonCollider2D.points.Length - 1];

            for (int i = 0; i < polygonCollider2D.points.Length - 1; ++i)
            {
                points[i] = polygonCollider2D.points[i];
            }

            return points;
        }

        private MinMax GetMinMax(Vector2[] points)
        {
            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;

            for (int i = 0; i < points.Length; ++i)
            {
                min = Vector2.Min(min, points[i]);
                max = Vector2.Max(max, points[i]);
            }

            return new MinMax(min, max);
        }
    }
#endif
}