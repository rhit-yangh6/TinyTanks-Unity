using UnityEngine;

namespace TerraformingTerrain2d
{
    public abstract class MarchingSquareMeshConfiguration
    {
        public readonly Vector2[] Vertices;
        public readonly int[] Triangles;
        public readonly Vector2[] UV;
        private readonly MarchingSquareInput _input;

        protected MarchingSquareMeshConfiguration(int verticesCount, int[] triangles, MarchingSquareInput input)
        {
            Vertices = new Vector2[verticesCount];
            UV = new Vector2[verticesCount];
            Triangles = triangles;
            _input = input;
        }

        public abstract void UpdateVertices(in MarchingSquare square);

        protected void CalculateUv(int index, Vector2 localUv)
        {
            UV[index] = (localUv + _input.GridIndex) / _input.Size;
        }
    }

    public class SquareMeshConfiguration0 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration0(MarchingSquareInput input) : base(0, new int[] { }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
        }
    }

    public class SquareMeshConfiguration1 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration1(MarchingSquareInput input) : base(3, new[] { 0, 1, 2 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.RightCentre;
            Vertices[2] = square.TopCentre;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, square.RightUV);
            CalculateUv(2, square.TopUV);
        }
    }

    public class SquareMeshConfiguration2 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration2(MarchingSquareInput input) : base(3, new[] { 0, 1, 2 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.RightCentre;
            Vertices[1] = square.BottomRight;
            Vertices[2] = square.BottomCentre;

            CalculateUv(0, square.RightUV);
            CalculateUv(1, Vector2.right);
            CalculateUv(2, square.BottomUV);
        }
    }

    public class SquareMeshConfiguration3 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration3(MarchingSquareInput input) : base(4, new[] { 0, 1, 2, 0, 2, 3 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.BottomRight;
            Vertices[2] = square.BottomCentre;
            Vertices[3] = square.TopCentre;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, Vector2.right);
            CalculateUv(2, square.BottomUV);
            CalculateUv(3, square.TopUV);
        }
    }

    public class SquareMeshConfiguration4 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration4(MarchingSquareInput input) : base(3, new[] { 0, 1, 2 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.BottomCentre;
            Vertices[1] = square.BottomLeft;
            Vertices[2] = square.LeftCentre;

            CalculateUv(0, square.BottomUV);
            CalculateUv(1, Vector2.zero);
            CalculateUv(2, square.LeftUV);
        }
    }

    public class SquareMeshConfiguration5 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration5(MarchingSquareInput input) : base(6,
            new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.RightCentre;
            Vertices[2] = square.BottomCentre;
            Vertices[3] = square.BottomLeft;
            Vertices[4] = square.LeftCentre;
            Vertices[5] = square.TopCentre;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, square.RightUV);
            CalculateUv(2, square.BottomUV);
            CalculateUv(3, Vector2.zero);
            CalculateUv(4, square.LeftUV);
            CalculateUv(5, square.TopUV);
        }
    }

    public class SquareMeshConfiguration6 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration6(MarchingSquareInput input) : base(4, new[] { 0, 1, 2, 0, 2, 3 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.BottomRight;
            Vertices[1] = square.BottomLeft;
            Vertices[2] = square.LeftCentre;
            Vertices[3] = square.RightCentre;

            CalculateUv(0, Vector2.right);
            CalculateUv(1, Vector2.zero);
            CalculateUv(2, square.LeftUV);
            CalculateUv(3, square.RightUV);
        }
    }

    public class SquareMeshConfiguration7 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration7(MarchingSquareInput input) : base(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, },
            input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.BottomRight;
            Vertices[2] = square.BottomLeft;
            Vertices[3] = square.LeftCentre;
            Vertices[4] = square.TopCentre;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, Vector2.right);
            CalculateUv(2, Vector2.zero);
            CalculateUv(3, square.LeftUV);
            CalculateUv(4, square.TopUV);
        }
    }

    public class SquareMeshConfiguration8 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration8(MarchingSquareInput input) : base(3, new[] { 0, 1, 2 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.LeftCentre;
            Vertices[1] = square.TopLeft;
            Vertices[2] = square.TopCentre;

            CalculateUv(0, square.LeftUV);
            CalculateUv(1, Vector2.up);
            CalculateUv(2, square.TopUV);
        }
    }

    public class SquareMeshConfiguration9 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration9(MarchingSquareInput input) : base(4, new[] { 0, 1, 2, 0, 2, 3 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.RightCentre;
            Vertices[2] = square.LeftCentre;
            Vertices[3] = square.TopLeft;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, square.RightUV);
            CalculateUv(2, square.LeftUV);
            CalculateUv(3, Vector2.up);
        }
    }

    public class SquareMeshConfiguration10 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration10(MarchingSquareInput input) : base(6,
            new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.RightCentre;
            Vertices[1] = square.BottomRight;
            Vertices[2] = square.BottomCentre;
            Vertices[3] = square.LeftCentre;
            Vertices[4] = square.TopLeft;
            Vertices[5] = square.TopCentre;

            CalculateUv(0, square.RightUV);
            CalculateUv(1, Vector2.right);
            CalculateUv(2, square.BottomUV);
            CalculateUv(3, square.LeftUV);
            CalculateUv(4, Vector2.up);
            CalculateUv(5, square.TopUV);
        }
    }

    public class SquareMeshConfiguration11 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration11(MarchingSquareInput input) : base(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, },
            input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.BottomRight;
            Vertices[2] = square.BottomCentre;
            Vertices[3] = square.LeftCentre;
            Vertices[4] = square.TopLeft;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, Vector2.right);
            CalculateUv(2, square.BottomUV);
            CalculateUv(3, square.LeftUV);
            CalculateUv(4, Vector2.up);
        }
    }

    public class SquareMeshConfiguration12 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration12(MarchingSquareInput input) : base(4, new[] { 0, 1, 2, 0, 2, 3 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.BottomCentre;
            Vertices[1] = square.BottomLeft;
            Vertices[2] = square.TopLeft;
            Vertices[3] = square.TopCentre;

            CalculateUv(0, square.BottomUV);
            CalculateUv(1, Vector2.zero);
            CalculateUv(2, Vector2.up);
            CalculateUv(3, square.TopUV);
        }
    }

    public class SquareMeshConfiguration13 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration13(MarchingSquareInput input) : base(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, },
            input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.RightCentre;
            Vertices[2] = square.BottomCentre;
            Vertices[3] = square.BottomLeft;
            Vertices[4] = square.TopLeft;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, square.RightUV);
            CalculateUv(2, square.BottomUV);
            CalculateUv(3, Vector2.zero);
            CalculateUv(4, Vector2.up);
        }
    }

    public class SquareMeshConfiguration14 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration14(MarchingSquareInput input) : base(5, new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, },
            input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.RightCentre;
            Vertices[1] = square.BottomRight;
            Vertices[2] = square.BottomLeft;
            Vertices[3] = square.TopLeft;
            Vertices[4] = square.TopCentre;

            CalculateUv(0, square.RightUV);
            CalculateUv(1, Vector2.right);
            CalculateUv(2, Vector2.zero);
            CalculateUv(3, Vector2.up);
            CalculateUv(4, square.TopUV);
        }
    }

    public class SquareMeshConfiguration15 : MarchingSquareMeshConfiguration
    {
        public SquareMeshConfiguration15(MarchingSquareInput input) : base(4, new[] { 0, 1, 2, 0, 2, 3 }, input)
        {
        }

        public override void UpdateVertices(in MarchingSquare square)
        {
            Vertices[0] = square.TopRight;
            Vertices[1] = square.BottomRight;
            Vertices[2] = square.BottomLeft;
            Vertices[3] = square.TopLeft;

            CalculateUv(0, Vector2.one);
            CalculateUv(1, Vector2.right);
            CalculateUv(2, Vector2.zero);
            CalculateUv(3, Vector2.up);
        }
    }
}