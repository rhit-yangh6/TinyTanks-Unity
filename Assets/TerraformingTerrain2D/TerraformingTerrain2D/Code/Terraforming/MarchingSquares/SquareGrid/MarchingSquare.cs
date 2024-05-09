using UnityEngine;

namespace TerraformingTerrain2d
{
    public struct MarchingSquare
    {
        public readonly Vector2 TopLeft;
        public readonly Vector2 TopRight;
        public readonly Vector2 BottomLeft;
        public readonly Vector2 BottomRight;
        private float _topUV;
        private float _leftUV;
        private float _rightUV;
        private float _bottomUV;

        public MarchingSquare(Vector2 position, float scale)
        {
            TopRight = position + scale * Vector2.one / 2f;
            BottomRight = TopRight + Vector2.down * scale;
            BottomLeft = BottomRight + Vector2.left * scale;
            TopLeft = BottomLeft + Vector2.up * scale;
            TopCentre = Vector2.zero;
            LeftCentre = Vector2.zero;
            RightCentre = Vector2.zero;
            BottomCentre = Vector2.zero;
            _topUV = 0;
            _leftUV = 0;
            _rightUV = 0;
            _bottomUV = 0;
        }

        public Vector2 TopCentre { get; private set; }
        public Vector2 LeftCentre { get; private set; }
        public Vector2 RightCentre { get; private set; }
        public Vector2 BottomCentre { get; private set; }

        public Vector2 TopUV => new(_topUV, 1);
        public Vector2 LeftUV => new(0, 1 - _leftUV);
        public Vector2 RightUV => new(1, 1 - _rightUV);
        public Vector2 BottomUV => new(_bottomUV, 0);

        public void Interpolate(MarchingSquareImmutableInput immutableInput)
        {
            _topUV = GetLerp(immutableInput.TopLeftLerp, immutableInput.TopRightLerp, immutableInput.IsoValue);
            _leftUV = GetLerp(immutableInput.TopLeftLerp, immutableInput.BottomLeftLerp, immutableInput.IsoValue);
            _rightUV = GetLerp(immutableInput.TopRightLerp, immutableInput.BottomRightLerp, immutableInput.IsoValue);
            _bottomUV = GetLerp(immutableInput.BottomLeftLerp, immutableInput.BottomRightLerp, immutableInput.IsoValue);

            TopCentre = Vector2.Lerp(TopLeft, TopRight, _topUV);
            LeftCentre = Vector2.Lerp(TopLeft, BottomLeft, _leftUV);
            RightCentre = Vector2.Lerp(TopRight, BottomRight, _rightUV);
            BottomCentre = Vector2.Lerp(BottomLeft, BottomRight, _bottomUV);
        }

        private float GetLerp(float start, float end, float isoValue)
        {
            float lerp = Mathf.InverseLerp(start, end, isoValue);

            return Mathf.Clamp01(lerp);
        }
    }
}