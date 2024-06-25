using UnityEngine;

namespace Destructible2D
{
	/// <summary>This class allows you to trim the edges of a destructible sprite's alphaData.</summary>
	public static class D2dTrim
	{
		private static Color32[] alphaData;
		private static int       alphaWidth;
		private static int       alphaHeight;

		public static void Trim(D2dDestructible destructible)
		{
			alphaData   = destructible.AlphaData;
			alphaWidth  = destructible.AlphaWidth;
			alphaHeight = destructible.AlphaHeight;

			var xMin = 0;
			var xMax = alphaWidth;
			var yMin = 0;
			var yMax = alphaHeight;

			for (var x = xMin; x < xMax; x++)
			{
				if (FastSolidAlphaVertical(yMin, yMax, x, alphaWidth) == false) xMin += 1; else break;
			}

			for (var x = xMax - 1; x >= xMin; x--)
			{
				if (FastSolidAlphaVertical(yMin, yMax, x, alphaWidth) == false) xMax -= 1; else break;
			}

			for (var y = yMin; y < yMax; y++)
			{
				if (FastSolidAlphaHorizontal(xMin, xMax, y, alphaWidth) == false) yMin += 1; else break;
			}

			for (var y = yMax - 1; y >= yMin; y--)
			{
				if (FastSolidAlphaHorizontal(xMin, xMax, y, alphaWidth) == false) yMax -= 1; else break;
			}

			var width  = xMax - xMin + 2;
			var height = yMax - yMin + 2;
			var rect   = D2dRect.CreateFromMinSize(xMin - 1, yMin - 1, width, height);

			D2dCommon.ReserveTempAlphaDataClear(width, height);

			D2dCommon.PasteAlpha(alphaData, alphaWidth, xMin, xMax, yMin, yMax, 1, 1, width);

			destructible.SubsetAlphaWith(D2dCommon.tempAlphaData, rect, destructible.AlphaCountRaw);
		}

		private static bool FastSolidAlphaHorizontal(int xMin, int xMax, int y, int alphaWidth)
		{
			var offset = y * alphaWidth;

            for (var x = xMin; x < xMax; x++)
			{
				if (alphaData[x + offset].a > 0)
				{
					return true;
				}
			}

			return false;
		}

		private static bool FastSolidAlphaVertical(int yMin, int yMax, int x, int alphaWidth)
		{
			for (var y = yMin; y < yMax; y++)
			{
                if (alphaData[x + y * alphaWidth].a > 0)
				{
					return true;
				}
			}

			return false;
		}
	}
}