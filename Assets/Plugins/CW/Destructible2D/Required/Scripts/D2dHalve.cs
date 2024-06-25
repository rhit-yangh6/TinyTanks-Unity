using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This class allows you to halve the width & height of the pixels in an alphaData.</summary>
	public static class D2dHalve
	{
		public static void Halve(ref Color32[] alphaData, ref int alphaWidth, ref int alphaHeight, ref Vector2 alphaOffset, ref Vector2 alphaScale)
		{
			var oldWidth  = alphaWidth;
			var oldHeight = alphaHeight;

			Halve(ref alphaData, ref alphaWidth, ref alphaHeight);

			var pixelW = CwHelper.Reciprocal(oldWidth ) * alphaScale.x;
			var pixelH = CwHelper.Reciprocal(oldHeight) * alphaScale.y;

			alphaOffset.x += pixelW * 0.5f;
			alphaOffset.y += pixelH * 0.5f;
			alphaScale.x  -= pixelW * 1.0f;
			alphaScale.y  -= pixelH * 1.0f;
		}

		public static void Halve(ref Color32[] alphaData, ref int alphaWidth, ref int alphaHeight)
		{
			var newWidth  = Mathf.Max(2, alphaWidth  / 2);
			var newHeight = Mathf.Max(2, alphaHeight / 2);
			var newData   = new Color32[newWidth * newHeight];
			var pixelW    = 1.0f / alphaWidth;
			var pixelH    = 1.0f / alphaHeight;
			var stepX     = (1.0f - pixelW * 2.0f) / (newWidth - 1);
			var stepY     = (1.0f - pixelH * 2.0f) / (newHeight - 1);

			for (var y = 0; y < newHeight; y++)
			{
				var newOffset = y * newWidth;

				for (var x = 0; x < newWidth; x++)
				{
					newData[x + newOffset] = GetBilinearFast(alphaData, x * stepX + pixelW, y * stepY + pixelH, alphaWidth, alphaHeight);
				}
			}

			alphaData   = newData;
			alphaWidth  = newWidth;
			alphaHeight = newHeight;
		}

		public static Color32 GetBilinearFast(Color32[] alphaData, float u, float v, int alphaWidth, int alphaHeight)
		{
			u = u * (alphaWidth - 1);
			v = v * (alphaHeight - 1);

			var x = Mathf.FloorToInt(u);
			var y = Mathf.FloorToInt(v);
			var s = u - x;
			var t = v - y;

			var bl = GetDefault(alphaData, x, y, alphaWidth);
			var br = GetDefault(alphaData, x + 1, y, alphaWidth);
			var tl = GetDefault(alphaData, x, y + 1, alphaWidth);
			var tr = GetDefault(alphaData, x + 1, y + 1, alphaWidth);

			var bb = Color32.LerpUnclamped(bl, br, s);
			var tt = Color32.LerpUnclamped(tl, tr, s);

			return Color32.LerpUnclamped(bb, tt, t);
		}

		public static Color32 GetDefault(Color32[] data, int x, int y, int width)
		{
			return data[x + y * width];
		}
	}
}