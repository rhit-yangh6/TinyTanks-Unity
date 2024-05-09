using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public struct MarchingSquareFunctionsList
    {
        private readonly Vector2 CalculateUv(Vector2 localUv, Vector2Int gridIndex, Vector2Int size)
        {
            return (localUv + gridIndex) / size;
        }

        public readonly void UpdateVertices(int index, ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            switch (index)
            {
                case 1:
                    UpdateVertices_1(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 2:
                    UpdateVertices_2(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 3:
                    UpdateVertices_3(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 4:
                    UpdateVertices_4(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 5:
                    UpdateVertices_5(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 6:
                    UpdateVertices_6(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 7:
                    UpdateVertices_7(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 8:
                    UpdateVertices_8(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 9:
                    UpdateVertices_9(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 10:
                    UpdateVertices_10(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 11:
                    UpdateVertices_11(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 12:
                    UpdateVertices_12(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 13:
                    UpdateVertices_13(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 14:
                    UpdateVertices_14(ref vertices, ref uv, in square, gridIndex, size);
                    break;

                case 15:
                    UpdateVertices_15(ref vertices, ref uv, in square, gridIndex, size);
                    break;
            }
        }

        private readonly void UpdateVertices_1(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.RightCentre;
            vertices[2] = square.TopCentre;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(square.RightUV, gridIndex, size);
            uv[2] = CalculateUv(square.TopUV, gridIndex, size);
        }

        private readonly void UpdateVertices_2(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.RightCentre;
            vertices[1] = square.BottomRight;
            vertices[2] = square.BottomCentre;

            uv[0] = CalculateUv(square.RightUV, gridIndex, size);
            uv[1] = CalculateUv(Vector2.right, gridIndex, size);
            uv[2] = CalculateUv(square.BottomUV, gridIndex, size);
        }

        private readonly void UpdateVertices_3(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.BottomRight;
            vertices[2] = square.BottomCentre;
            vertices[3] = square.TopCentre;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(Vector2.right, gridIndex, size);
            uv[2] = CalculateUv(square.BottomUV, gridIndex, size);
            uv[3] = CalculateUv(square.TopUV, gridIndex, size);
        }

        private readonly void UpdateVertices_4(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.BottomCentre;
            vertices[1] = square.BottomLeft;
            vertices[2] = square.LeftCentre;

            uv[0] = CalculateUv(square.BottomUV, gridIndex, size);
            uv[1] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[2] = CalculateUv(square.LeftUV, gridIndex, size);
        }

        private readonly void UpdateVertices_5(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.RightCentre;
            vertices[2] = square.BottomCentre;
            vertices[3] = square.BottomLeft;
            vertices[4] = square.LeftCentre;
            vertices[5] = square.TopCentre;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(square.RightUV, gridIndex, size);
            uv[2] = CalculateUv(square.BottomUV, gridIndex, size);
            uv[3] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[4] = CalculateUv(square.LeftUV, gridIndex, size);
            uv[5] = CalculateUv(square.TopUV, gridIndex, size);
        }

        private readonly void UpdateVertices_6(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.BottomRight;
            vertices[1] = square.BottomLeft;
            vertices[2] = square.LeftCentre;
            vertices[3] = square.RightCentre;

            uv[0] = CalculateUv(Vector2.right, gridIndex, size);
            uv[1] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[2] = CalculateUv(square.LeftUV, gridIndex, size);
            uv[3] = CalculateUv(square.RightUV, gridIndex, size);
        }

        private readonly void UpdateVertices_7(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.BottomRight;
            vertices[2] = square.BottomLeft;
            vertices[3] = square.LeftCentre;
            vertices[4] = square.TopCentre;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(Vector2.right, gridIndex, size);
            uv[2] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[3] = CalculateUv(square.LeftUV, gridIndex, size);
            uv[4] = CalculateUv(square.TopUV, gridIndex, size);
        }

        private readonly void UpdateVertices_8(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.LeftCentre;
            vertices[1] = square.TopLeft;
            vertices[2] = square.TopCentre;

            uv[0] = CalculateUv(square.LeftUV, gridIndex, size);
            uv[1] = CalculateUv(Vector2.up, gridIndex, size);
            uv[2] = CalculateUv(square.TopUV, gridIndex, size);
        }


        private readonly void UpdateVertices_9(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.RightCentre;
            vertices[2] = square.LeftCentre;
            vertices[3] = square.TopLeft;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(square.RightUV, gridIndex, size);
            uv[2] = CalculateUv(square.LeftUV, gridIndex, size);
            uv[3] = CalculateUv(Vector2.up, gridIndex, size);
        }

        private readonly void UpdateVertices_10(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.RightCentre;
            vertices[1] = square.BottomRight;
            vertices[2] = square.BottomCentre;
            vertices[3] = square.LeftCentre;
            vertices[4] = square.TopLeft;
            vertices[5] = square.TopCentre;

            uv[0] = CalculateUv(square.RightUV, gridIndex, size);
            uv[1] = CalculateUv(Vector2.right, gridIndex, size);
            uv[2] = CalculateUv(square.BottomUV, gridIndex, size);
            uv[3] = CalculateUv(square.LeftUV, gridIndex, size);
            uv[4] = CalculateUv(Vector2.up, gridIndex, size);
            uv[5] = CalculateUv(square.TopUV, gridIndex, size);
        }

        private readonly void UpdateVertices_11(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.BottomRight;
            vertices[2] = square.BottomCentre;
            vertices[3] = square.LeftCentre;
            vertices[4] = square.TopLeft;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(Vector2.right, gridIndex, size);
            uv[2] = CalculateUv(square.BottomUV, gridIndex, size);
            uv[3] = CalculateUv(square.LeftUV, gridIndex, size);
            uv[4] = CalculateUv(Vector2.up, gridIndex, size);
        }

        private readonly void UpdateVertices_12(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.BottomCentre;
            vertices[1] = square.BottomLeft;
            vertices[2] = square.TopLeft;
            vertices[3] = square.TopCentre;

            uv[0] = CalculateUv(square.BottomUV, gridIndex, size);
            uv[1] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[2] = CalculateUv(Vector2.up, gridIndex, size);
            uv[3] = CalculateUv(square.TopUV, gridIndex, size);
        }

        private readonly void UpdateVertices_13(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.RightCentre;
            vertices[2] = square.BottomCentre;
            vertices[3] = square.BottomLeft;
            vertices[4] = square.TopLeft;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(square.RightUV, gridIndex, size);
            uv[2] = CalculateUv(square.BottomUV, gridIndex, size);
            uv[3] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[4] = CalculateUv(Vector2.up, gridIndex, size);
        }

        private readonly void UpdateVertices_14(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.RightCentre;
            vertices[1] = square.BottomRight;
            vertices[2] = square.BottomLeft;
            vertices[3] = square.TopLeft;
            vertices[4] = square.TopCentre;

            uv[0] = CalculateUv(square.RightUV, gridIndex, size);
            uv[1] = CalculateUv(Vector2.right, gridIndex, size);
            uv[2] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[3] = CalculateUv(Vector2.up, gridIndex, size);
            uv[4] = CalculateUv(square.TopUV, gridIndex, size);
        }

        private readonly void UpdateVertices_15(ref Span<Vector2> vertices, ref Span<Vector2> uv,
            in MarchingSquare square, Vector2Int gridIndex, Vector2Int size)
        {
            vertices[0] = square.TopRight;
            vertices[1] = square.BottomRight;
            vertices[2] = square.BottomLeft;
            vertices[3] = square.TopLeft;

            uv[0] = CalculateUv(Vector2.one, gridIndex, size);
            uv[1] = CalculateUv(Vector2.right, gridIndex, size);
            uv[2] = CalculateUv(Vector2.zero, gridIndex, size);
            uv[3] = CalculateUv(Vector2.up, gridIndex, size);
        }
    }
}