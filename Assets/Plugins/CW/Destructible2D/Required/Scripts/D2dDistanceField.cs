using UnityEngine;

namespace Destructible2D
{
	/// <summary>This component allows you to calculate a distance field from an array of pixels.</summary>
	public class D2dDistanceField
	{
		public struct Cell
		{
			public int x;
			public int y;
			public int d;

			public Cell(int newX, int newY, int newD)
			{
				x = newX;
				y = newY;
				d = newD;
			}
		}

		public D2dRect Rect;

		public Cell[] Cells;

		public void Clear()
		{
			Rect.Clear();
		}

		public void Prepare(D2dRect newRect, bool fill)
		{
			var s = newRect.Area;

			Rect = newRect;

			if (Cells == null || Cells.Length < s)
			{
				Cells = new Cell[s];
			}

			if (fill == true)
			{
				for (var i = 0; i < s; i++)
				{
					Cells[i] = new Cell(10000, 10000, 10000 * 10000);
				}
			}
		}

		public void Transform(D2dRect newRect, int width, int height, Color32[] data)
		{
			Clear();
			Prepare(newRect, false);

			for (var y = newRect.MinY; y < newRect.MaxY; y++)
			{
				for (var x = newRect.MinX; x < newRect.MaxX; x++)
				{
					var alpha = data[x + y * width].a;
					var index = x - newRect.MinX + (y - newRect.MinY) * newRect.SizeX;

					if (alpha > 127)
					{
						Cells[index] = new Cell(0, 0, 0);
					}
					else
					{
						Cells[index] = new Cell(10000, 10000, 10000 * 10000);
					}
				}
			}

			Transform();
		}

		public void Transform(D2dRect newRect, D2dSplitter.Island island)
		{
			Clear();
			Prepare(newRect, true);

			for (var i = island.Lines.Count - 1; i >= 0; i--)
			{
				var line = island.Lines[i];

				for (var x = line.MinX; x < line.MaxX; x++)
				{
					Cells[x - newRect.MinX + (line.Y - newRect.MinY) * newRect.SizeX] = new Cell(0, 0, 0);
				}
			}

			Transform();
		}

		private Cell GetF(int x, int y)
		{
			return Cells[x + y * Rect.SizeX];
		}

		private void SetF(int x, int y, Cell p)
		{
			Cells[x + y * Rect.SizeX] = p;
		}

		private Cell Get(int x, int y)
		{
			if (x >= 0 && y >= 0 && x < Rect.SizeX && y < Rect.SizeY)
			{
				return Cells[x + y * Rect.SizeX];
			}

			return new Cell(10000, 10000, 10000 * 10000);
		}

		void Compare(ref Cell p, int x, int y, int offsetx, int offsety)
		{
			var other = Get(x + offsetx, y + offsety);
			other.x += offsetx;
			other.y += offsety;
			other.d = other.x * other.x + other.y * other.y;

			if (other.d < p.d)
			{
				p.x = other.x;
				p.y = other.y;
				p.d = other.d;
			}
		}

		private void Transform()
		{
			var w = Rect.SizeX;
			var h = Rect.SizeY;

			for (var y = 0; y < h; y++)
			{
				for (var x = 0; x < w; x++)
				{
					var p = GetF(x, y);
					Compare(ref p, x, y, -1,  0);
					Compare(ref p, x, y,  0, -1);
					Compare(ref p, x, y, -1, -1);
					Compare(ref p, x, y,  1, -1);
					SetF(x, y, p);
				}

				for (var x = w - 1; x >= 0; x--)
				{
					var p = GetF(x, y);
					Compare(ref p, x, y, 1, 0);
					SetF(x, y, p);
				}
			}

			for (var y = h - 1; y >= 0; y--)
			{
				for (var x = w - 1; x >= 0; x--)
				{
					var p = GetF(x, y);
					Compare(ref p, x, y,  1,  0);
					Compare(ref p, x, y,  0,  1);
					Compare(ref p, x, y, -1,  1);
					Compare(ref p, x, y,  1,  1);
					SetF(x, y, p);
				}

				for (var x = 0; x < w; x++)
				{
					var p = GetF(x, y);
					Compare(ref p, x, y, -1, 0);
					SetF(x, y, p);
				}
			}
		}
	}
}