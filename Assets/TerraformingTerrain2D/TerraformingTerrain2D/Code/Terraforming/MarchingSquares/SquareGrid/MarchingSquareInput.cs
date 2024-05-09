using UnityEngine;

namespace TerraformingTerrain2d
{
    public class MarchingSquareInput
    {
        public readonly ScalarField ScalarField;
        public readonly Vector2Int Size;
        private readonly IsoValue _isoValue;

        public MarchingSquareInput(ScalarField scalarField, Vector2Int size, IsoValue isoValue)
        {
            ScalarField = scalarField;
            _isoValue = isoValue;
            Size = size;
        }

        public float IsoValue => _isoValue.Value;
        public float TopRightLerp { get; private set; }
        public float BottomRightLerp { get; private set; }
        public float BottomLeftLerp { get; private set; }
        public float TopLeftLerp { get; private set; }
        public Vector2Int GridIndex { get; private set; }

        public void Update(int i, int j)
        {
            TopRightLerp = ScalarField[i + 1, j + 1];
            BottomRightLerp = ScalarField[i + 1, j];
            BottomLeftLerp = ScalarField[i, j];
            TopLeftLerp = ScalarField[i, j + 1];
            GridIndex = new Vector2Int(i, j);
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

        public MarchingSquareImmutableInput ToImmutable()
        {
            return new MarchingSquareImmutableInput(IsoValue, TopRightLerp, BottomRightLerp, BottomLeftLerp,
                TopLeftLerp);
        }
    }
}