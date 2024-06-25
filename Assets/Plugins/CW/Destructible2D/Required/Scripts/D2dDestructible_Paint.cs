using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	// This class handles the various ways to modify the destruction state of a D2dDestructible.
	public partial class D2dDestructible
	{
		public enum PaintType
		{
			Cut,
			Heal,
			Subtract,
			SubtractInvColor,
			CutMinRGB,
			AlphaBlendRGB
		}

		/// <summary>This paints the current destructible with the specified paint type, at the specified matrix, with the specified shape.</summary>
		public void Paint(PaintType paint, Matrix4x4 matrix, D2dShape shape, Color tint)
		{
			switch (paint)
			{
				case PaintType.Cut: Cut(matrix, shape, tint); break;
				case PaintType.Heal: Heal(matrix, shape, tint); break;
				case PaintType.Subtract: Subtract(matrix, shape, tint); break;
				case PaintType.SubtractInvColor: Subtract(matrix, shape, new Color(1.0f - tint.r, 1.0f - tint.g, 1.0f - tint.b, tint.a)); break;
				//case PaintType.SubtractInvColor: SubtractInvRGB(matrix, shape, color); break;
				case PaintType.CutMinRGB: CutMinRGB(matrix, shape, tint); break;
				case PaintType.AlphaBlendRGB: AlphaBlendRGB(matrix, shape, tint); break;
			}
		}

		private static Matrix4x4 matrix;
		private static Matrix4x4 inverse;

		private static D2dRect   rect;
		private static int       shapeW;
		private static int       shapeH;
		private static Texture2D shapeColor;
		private static Texture2D shapeAlpha;
		private static Color     shapeTint;
		private static Vector3   shapeCoordA;
		private static Vector3   shapeCoordB;
		private static float     alphaPixelX;
		private static float     alphaPixelY;
		private static float     alphaHalfPixelX;
		private static float     alphaHalfPixelY;

		private float GetSolidStrength(byte alpha)
		{
			return System.Math.Min(1.0f, (255.0f - alpha) / solidRange);
		}

		private bool BeginCommon(Matrix4x4 newMatrix, Texture2D newColor, Texture2D newAlpha, Color newTint)
		{
			if (ready == true && newColor != null && newAlpha != null && newColor.width == newAlpha.width && newColor.height == newAlpha.height)
			{
				rect       = default(D2dRect);
				matrix     = WorldToAlphaMatrix * newMatrix;
				alphaCount = -1;

				if (D2dCommon.CalculateRect(matrix, ref rect, alphaWidth, alphaHeight) == true)
				{
					inverse    = matrix.inverse;
					shapeColor = newColor;
					shapeAlpha = newAlpha;
					shapeW     = newColor.width;
					shapeH     = newColor.height;
					shapeTint  = newTint * paintMultiplier;

					rect.MinX = Mathf.Clamp(rect.MinX, 0, alphaWidth );
					rect.MaxX = Mathf.Clamp(rect.MaxX, 0, alphaWidth );
					rect.MinY = Mathf.Clamp(rect.MinY, 0, alphaHeight);
					rect.MaxY = Mathf.Clamp(rect.MaxY, 0, alphaHeight);

					alphaPixelX     = CwHelper.Reciprocal(alphaWidth );
					alphaPixelY     = CwHelper.Reciprocal(alphaHeight);
					alphaHalfPixelX = alphaPixelX * 0.5f;
					alphaHalfPixelY = alphaPixelY * 0.5f;

					modifiedPixels.Clear();

					return true;
				}
			}

			return false;
		}

		public void Cut(Matrix4x4 newMatrix, D2dShape newShape, Color newTint)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape.Alpha, newShape.Alpha, newTint) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index        = offset + x;
							var alphaPixel   = alphaData[index];
							var alphaOld     = alphaPixel.a;
							var sampledAlpha = SampleShapeAlpha();

							if (solidRange > 0)
							{
								sampledAlpha = (byte)(sampledAlpha * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - sampledAlpha, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		public void Heal(Matrix4x4 newMatrix, D2dShape newShape, Color newTint)
		{
			if (indestructible == false && CanHeal == true && BeginCommon(newMatrix, newShape.Alpha, newShape.Alpha, newTint) == true)
			{
				var healData = healSnapshot.Data;
				
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index        = offset + x;
							var alphaPixel   = alphaData[index];
							var alphaOld     = alphaPixel.a;
							var sampledAlpha = SampleShapeAlpha();

							alphaPixel.a = (byte)System.Math.Min(alphaPixel.a + sampledAlpha, healData.AlphaData[index].a);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}
		
		public void Subtract(Matrix4x4 newMatrix, D2dShape newShape, Color newTint)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape.Color, newShape.Alpha, newTint) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index        = offset + x;
							var alphaPixel   = alphaData[index];
							var alphaOld     = alphaPixel.a;
							var sampledColor = SampleShapeColor();
							var sampledAlpha = SampleShapeAlpha();

							if (solidRange > 0)
							{
								sampledAlpha = (byte)(sampledAlpha * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.r = (byte)System.Math.Max(alphaPixel.r - sampledColor.r, 0);
							alphaPixel.g = (byte)System.Math.Max(alphaPixel.g - sampledColor.g, 0);
							alphaPixel.b = (byte)System.Math.Max(alphaPixel.b - sampledColor.b, 0);
							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - sampledAlpha  , 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		public void SubtractInvRGB(Matrix4x4 newMatrix, D2dShape newShape, Color newTint)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape.Color, newShape.Alpha, newTint) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index        = offset + x;
							var alphaPixel   = alphaData[index];
							var alphaOld     = alphaPixel.a;
							var sampledColor = SampleShapeColor();
							var sampledAlpha = SampleShapeAlpha();

							if (solidRange > 0)
							{
								sampledAlpha = (byte)(sampledAlpha * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.r = (byte)System.Math.Max(alphaPixel.r - (255 - sampledColor.r), 0);
							alphaPixel.g = (byte)System.Math.Max(alphaPixel.g - (255 - sampledColor.g), 0);
							alphaPixel.b = (byte)System.Math.Max(alphaPixel.b - (255 - sampledColor.b), 0);
							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - sampledAlpha, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		public void CutMinRGB(Matrix4x4 newMatrix, D2dShape newShape, Color newTint)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape.Color, newShape.Alpha, newTint) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index        = offset + x;
							var alphaPixel   = alphaData[index];
							var alphaOld     = alphaPixel.a;
							var sampledColor = SampleShapeColor();
							var sampledAlpha = SampleShapeAlpha();

							if (solidRange > 0)
							{
								sampledAlpha = (byte)(sampledAlpha * GetSolidStrength(alphaPixel.a));
							}

							alphaPixel.r = System.Math.Min(alphaPixel.r, sampledColor.r);
							alphaPixel.g = System.Math.Min(alphaPixel.g, sampledColor.g);
							alphaPixel.b = System.Math.Min(alphaPixel.b, sampledColor.b);
							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - sampledAlpha, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		public void AlphaBlendRGB(Matrix4x4 newMatrix, D2dShape newShape, Color newTint)
		{
			if (indestructible == false && BeginCommon(newMatrix, newShape.Color, newShape.Alpha, newTint) == true)
			{
				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var v      = y * alphaPixelY + alphaHalfPixelY;
					var offset = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var u = x * alphaPixelX + alphaHalfPixelX;

						if (CalculateShapeCoord(u, v) == true)
						{
							var index        = offset + x;
							var alphaPixel   = alphaData[index];
							var alphaOld     = alphaPixel.a;
							var sampledColor = SampleShapeColor();
							var sampledAlpha = SampleShapeAlpha();

							if (solidRange > 0)
							{
								sampledAlpha = (byte)(sampledAlpha * GetSolidStrength(alphaPixel.a));
							}

							var current = (Color)alphaPixel;
							var color   = (Color)sampledColor;

							var str = 1.0f - color.a;
							var div = color.a + current.a * str;

							if (div > 0.0f)
							{
								current.r = (color.r * color.a + current.r * current.a * str) / div;
								current.g = (color.g * color.a + current.g * current.a * str) / div;
								current.b = (color.b * color.a + current.b * current.a * str) / div;

								alphaPixel = current;
							}

							alphaPixel.a = (byte)System.Math.Max(alphaPixel.a - sampledAlpha, 0);

							WriteCommon(index, alphaOld, alphaPixel);
						}
					}
				}

				EndCommon();
			}
		}

		private void WriteCommon(int index, byte alphaOld, Color32 alphaNew)
		{
			if (pixels == PixelsType.PixelatedBinary)
			{
				alphaNew.a = alphaNew.a > 127 ? (byte)255 : (byte)0;
			}

			if (monitorPixels == true)
			{
				var binaryA = alphaOld   > 127;
				var binaryB = alphaNew.a > 127;

				if (binaryA != binaryB)
				{
					modifiedPixels.Add(index);
				}
			}

			alphaData[index] = alphaNew;
		}

		private void EndCommon()
		{
			if (monitorPixels == true && modifiedPixels.Count > 0)
			{
				if (OnModifiedPixels != null)
				{
					OnModifiedPixels.Invoke(modifiedPixels);
				}

				if (OnGlobalModifiedPixels != null)
				{
					OnGlobalModifiedPixels.Invoke(this, modifiedPixels);
				}
			}

			AlphaModified.Add(rect);
		}

		private void CheckPixelChange(int i, byte a, byte b)
		{
			
		}

		private Color32 SampleShapeColor()
		{
			var x = (int)(shapeCoordB.x * shapeW);
			var y = (int)(shapeCoordB.y * shapeH);

			return shapeColor.GetPixel(x, y) * shapeTint;
		}

		private byte SampleShapeAlpha()
		{
			var x = (int)(shapeCoordB.x * shapeW);
			var y = (int)(shapeCoordB.y * shapeH);

			return (byte)(shapeAlpha.GetPixel(x, y).a * shapeTint.a * 255.0f);
		}

		private static bool CalculateShapeCoord(float u, float v)
		{
			shapeCoordA.x = u;
			shapeCoordA.y = v;

			shapeCoordB = inverse.MultiplyPoint(shapeCoordA);

			return shapeCoordB.x >= 0.0f && shapeCoordB.x < 1.0f && shapeCoordB.y >= 0.0f && shapeCoordB.y < 1.0f;
		}
	}
}