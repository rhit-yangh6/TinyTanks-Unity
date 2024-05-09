
namespace TerraformingTerrain2d
{
    public readonly ref struct MarchingSquareImmutableInput
    {
        public readonly float IsoValue;
        public readonly float TopRightLerp;
        public readonly float BottomRightLerp;
        public readonly float BottomLeftLerp;
        public readonly float TopLeftLerp;

        public MarchingSquareImmutableInput(float isoValue, float topRightLerp, float bottomRightLerp,
            float bottomLeftLerp, float topLeftLerp)
        {
            IsoValue = isoValue;
            TopRightLerp = topRightLerp;
            BottomRightLerp = bottomRightLerp;
            BottomLeftLerp = bottomLeftLerp;
            TopLeftLerp = topLeftLerp;
        }

        public bool IsFullQuad => GetConfiguration() == 1 + 2 + 4 + 8;

        public int GetConfiguration()
        {
            int result = 0;

            if (IsoValue >= TopRightLerp)
                result += 1;

            if (IsoValue >= BottomRightLerp)
                result += 2;

            if (IsoValue >= BottomLeftLerp)
                result += 4;

            if (IsoValue >= TopLeftLerp)
                result += 8;

            return result;
        }
    }
}