using UnityEngine;

namespace Destructible2D
{
	/// <summary>This class allows you to blur a D2dDestructible's alphaData.</summary>
	public static class D2dBlur
	{
		public static Color32 colorA;
		public static Color32 colorB;
		public static Color32 colorC;

		public static void Blur(D2dDestructible destructible)
		{
			var alphaData   = destructible.AlphaData;
			var alphaWidth  = destructible.AlphaWidth;
			var alphaHeight = destructible.AlphaHeight;

			D2dCommon.ReserveTempAlphaData(alphaWidth, alphaHeight);

			BlurHorizontally(alphaData, D2dCommon.tempAlphaData, alphaWidth, alphaHeight);

			BlurVertically(D2dCommon.tempAlphaData, alphaData, alphaWidth, alphaHeight);
		}

		public static void BlurHorizontally(Color32[] src, Color32[] dst, int alphaWidth, int alphaHeight)
		{
			for (var y = 0; y < alphaHeight; y++)
			{
				var o = y * alphaWidth;

				colorA = src[o];
				colorB = src[o + 1];
				colorC = default(Color32);

				dst[o] = CombineColorsAB();

				o += 1;

				for (var x = 1; x < alphaWidth - 1; x++)
				{
					colorC = src[o + 1];

					dst[o] = CombineColorsABC();

					o += 1;

					colorA  = colorB;
					colorB  = colorC;
				}

				colorC = default(Color32);

				dst[o] = CombineColorsAB();
			}
		}

		public static void BlurVertically(Color32[] src, Color32[] dst, int alphaWidth, int alphaHeight)
		{
			for (var x = 0; x < alphaWidth; x++)
			{
				var o = x;

				colorA = src[o];
				colorB = src[o + alphaWidth];
				colorC = default(Color32);

				dst[o] = CombineColorsAB();

				o += alphaWidth;

				for (var y = 1; y < alphaHeight - 1; y++)
				{
					colorC = src[o + alphaWidth];

					dst[o] = CombineColorsABC();

					o += alphaWidth;

					colorA  = colorB;
					colorB  = colorC;
				}

				colorC = default(Color32);

				dst[o] = CombineColorsAB();
			}
		}

		private static Color32 CombineColorsAB()
		{
			var o = default(Color32);
			o.r = (byte)((colorA.r + colorB.r) / 3);
			o.g = (byte)((colorA.g + colorB.g) / 3);
			o.b = (byte)((colorA.b + colorB.b) / 3);
			o.a = (byte)((colorA.a + colorB.a) / 3);
			return o;
		}

		private static Color32 CombineColorsABC()
		{
			var o = default(Color32);
			o.r = (byte)((colorA.r + colorB.r + colorC.r) / 3);
			o.g = (byte)((colorA.g + colorB.g + colorC.g) / 3);
			o.b = (byte)((colorA.b + colorB.b + colorC.b) / 3);
			o.a = (byte)((colorA.a + colorB.a + colorC.a) / 3);
			return o;
		}
	}
}