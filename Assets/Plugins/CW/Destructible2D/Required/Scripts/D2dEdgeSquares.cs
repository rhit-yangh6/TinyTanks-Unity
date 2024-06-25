using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	/// <summary>This allows you to calculate the edges of a specific alphaData area.</summary>
	public static class D2dEdgeSquares
	{
		enum Side
		{
			W,
			E,
			S,
			N
		}

		class Square
		{
			public int   X;
			public int   Y;
			public int   Mask;
			public Point PointW;
			public Point PointE;
			public Point PointS;
			public Point PointN;
		}

		struct Point
		{
			public int x;
			public int y;

			public Point(int newX, int newY)
			{
				x = newX;
				y = newY;
			}

			public static Point operator - (Point a, Point b) { a.x -= b.x; a.y -= b.y; return a; }
			public static implicit operator Vector2(Point a) { return new Vector2(a.x, a.y); }
		}

		private static List<Square> squares = new List<Square>();

		private static List<Square> activeSquares = new List<Square>();

		public static Color32[] AlphaD; // Raw data
		public static int       AlphaW; // Width
		public static int       AlphaH; // Height
		public static int       MinX;
		public static int       MaxX;
		public static int       MinY;
		public static int       MaxY;

		private static int squaresH;
		private static int squaresV;

		private static LinkedList<Point> points = new LinkedList<Point>();

		private static List<Vector2> weldedPoints = new List<Vector2>();

		private static byte GetPolyAlpha(int x, int y)
		{
			return AlphaD[x + y * AlphaW].a;
		}

		private static int[,] subSquares = new int[256, 256];

		static D2dEdgeSquares()
		{
			for (var i = 0; i < 256; i++)
			{
				for (var j = 0; j < 256; j++)
				{
					var l = i - 128;
					var r = i - j;

					if (r != 0)
					{
						subSquares[i, j] = (l * 255) / r;
						//subSquares[i, j] = 255 * (l / (float)r);
					}
				}
			}
		}

		public static void CalculateCells()
		{
			squaresH = MaxX - MinX - 1;
			squaresV = MaxY - MinY - 1;

			var reserve = squaresH * squaresV - squares.Count; for (var i = reserve; i > 0; i--) squares.Add(new Square());
			var index   = 0;

			activeSquares.Clear();

			for (var y = MinY; y < MaxY - 1; y++)
			{
				var bl = GetPolyAlpha(MinX, y    ); var useBl = bl >= 128;
				var tl = GetPolyAlpha(MinX, y + 1); var useTl = tl >= 128;

				for (var x = MinX; x < MaxX - 1; x++)
				{
					var square = squares[index++];
					var br     = GetPolyAlpha(x + 1, y    ); var useBr = br >= 128;
					var tr     = GetPolyAlpha(x + 1, y + 1); var useTr = tr >= 128;
					var mask   = ((useBl ? 1 : 0) + (useBr ? 2 : 0) + (useTl ? 4 : 0) + (useTr ? 8 : 0)) % 15;

					if (mask > 0)
					{
						var xp = x * 255;
						var yp = y * 255;

						square.Mask = mask;
						square.X    = x - MinX;
						square.Y    = y - MinY;

						if (mask == 6 || mask == 9)
						{
							activeSquares.Add(square);
						}

						activeSquares.Add(square);

						if (useBl ^ useBr) square.PointS = new Point(xp + subSquares[bl, br], yp      );
						if (useTl ^ useTr) square.PointN = new Point(xp + subSquares[tl, tr], yp + 255);
						if (useBl ^ useTl) square.PointW = new Point(xp      , yp + subSquares[bl, tl]);
						if (useBr ^ useTr) square.PointE = new Point(xp + 255, yp + subSquares[br, tr]);
					}
					else
					{
						square.Mask = 0;
					}

					bl = br; useBl = useBr;
					tl = tr; useTl = useTr;
				}
			}
		}

		private static void DoTrace(Square square)
		{
			switch (square.Mask)
			{
				case 1 : square.Mask = 0; DoTrace(square, Side.W, true); DoTrace(square, Side.S, false); break;
				case 2 : square.Mask = 0; DoTrace(square, Side.S, true); DoTrace(square, Side.E, false); break;
				case 3 : square.Mask = 0; DoTrace(square, Side.W, true); DoTrace(square, Side.E, false); break;
				case 4 : square.Mask = 0; DoTrace(square, Side.N, true); DoTrace(square, Side.W, false); break;
				case 5 : square.Mask = 0; DoTrace(square, Side.N, true); DoTrace(square, Side.S, false); break;
				case 6 : square.Mask = 2; DoTrace(square, Side.N, true); DoTrace(square, Side.W, false); break;
				case 7 : square.Mask = 0; DoTrace(square, Side.N, true); DoTrace(square, Side.E, false); break;
				case 8 : square.Mask = 0; DoTrace(square, Side.E, true); DoTrace(square, Side.N, false); break;
				case 9 : square.Mask = 1; DoTrace(square, Side.E, true); DoTrace(square, Side.N, false); break;
				case 10: square.Mask = 0; DoTrace(square, Side.S, true); DoTrace(square, Side.N, false); break;
				case 11: square.Mask = 0; DoTrace(square, Side.W, true); DoTrace(square, Side.N, false); break;
				case 12: square.Mask = 0; DoTrace(square, Side.E, true); DoTrace(square, Side.W, false); break;
				case 13: square.Mask = 0; DoTrace(square, Side.E, true); DoTrace(square, Side.S, false); break;
				case 14: square.Mask = 0; DoTrace(square, Side.S, true); DoTrace(square, Side.W, false); break;
			}
		}

		private static void DoTrace(Square square, Side side, bool end)
		{
			switch (side)
			{
				case Side.W: AddPoint(square.PointW, end); DoTrace(square.X - 1, square.Y    , Side.E, end); break;
				case Side.E: AddPoint(square.PointE, end); DoTrace(square.X + 1, square.Y    , Side.W, end); break;
				case Side.S: AddPoint(square.PointS, end); DoTrace(square.X    , square.Y - 1, Side.N, end); break;
				case Side.N: AddPoint(square.PointN, end); DoTrace(square.X    , square.Y + 1, Side.S, end); break;
			}
		}

		private static void AddPoint(Point point, bool end)
		{
			if (end == true)
			{
				points.AddLast(point);
			}
			else
			{
				points.AddFirst(point);
			}
		}

		private static void DoTrace(int x, int y, Side side, bool end)
		{
			while (true)
			{
				if (x < 0 || y < 0 || x >= squaresH || y >= squaresV)
				{
					return;
				}

				var square = squares[x + y * squaresH];

				switch (square.Mask)
				{
					case 0: return;

					case 1:
						if (side == Side.W)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.S)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;

					case 2:
						if (side == Side.S)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.E)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 3:
						if (side == Side.W)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.E)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 4:
						if (side == Side.N)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.W)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 5:
						if (side == Side.N)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.S)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 6:
						if (side == Side.N)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 2; continue;
						}
						if (side == Side.W)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 2; continue;
						}
						if (side == Side.S)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 4; continue;
						}
						if (side == Side.E)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 4; continue;
						}
					break;
					case 7:
						if (side == Side.N)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.E)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 8:
						if (side == Side.E)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.N)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 9:
						if (side == Side.E)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 1; continue;
						}
						if (side == Side.N)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 1; continue;
						}
						if (side == Side.W)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 8; continue;
						}
						if (side == Side.S)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 8; continue;
						}
						break;
					case 10:
						if (side == Side.S)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.N)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 11:
						if (side == Side.W)
						{
							ShiftN(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.N)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 12:
						if (side == Side.E)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.W)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 13:
						if (side == Side.E)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.S)
						{
							ShiftE(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
					case 14:
						if (side == Side.S)
						{
							ShiftW(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
						if (side == Side.W)
						{
							ShiftS(square, end, ref x, ref y, ref side); square.Mask = 0; continue;
						}
					break;
				}

				return;
			}
		}

		private static void ShiftN(Square square, bool end, ref int x, ref int y, ref Side s)
		{
			AddPoint(square.PointN, end); y++; s = Side.S;
		}

		private static void ShiftS(Square square, bool end, ref int x, ref int y, ref Side s)
		{
			AddPoint(square.PointS, end); y--; s = Side.N;
		}

		private static void ShiftE(Square square, bool end, ref int x, ref int y, ref Side s)
		{
			AddPoint(square.PointE, end); x++; s = Side.W;
		}

		private static void ShiftW(Square square, bool end, ref int x, ref int y, ref Side s)
		{
			AddPoint(square.PointW, end); x--; s = Side.E;
		}

		private static Vector2[] Trace(Square square, float straighten)
		{
			points.Clear();

			DoTrace(square);

			if (points.Count > 2)
			{
				var point = points.First.Next;
				var total = 0.0f;

				while (point != points.Last)
				{
					var pointA  = (Vector2)point.Previous.Value;
					var pointB  = (Vector2)point.Value;
					var pointC  = (Vector2)point.Next.Value;
					var vectorA = (pointB - pointA).normalized;
					var vectorB = (pointC - pointB).normalized;
					var next    = point.Next;
					var error   = 1.0f - Vector2.Dot(vectorA, vectorB);

					total += error;

					if (total < straighten)
					{
						points.Remove(point);
					}
					else
					{
						total = 0.0f;
					}
					
					point = next;
				}
			}

			weldedPoints.Clear();

			foreach (var point in points)
			{
				weldedPoints.Add(point);
			}


			return D2dCache.CachedList<Vector2>.ToArray(weldedPoints);
		}

		public static void Build(D2dEdgeCollider.Cell cell, D2dEdgeCollider edgeCollider)
		{
			D2dCache.CachedList<Vector2>.Clear();

			for (var i = 0; i < activeSquares.Count; i++)
			{
				var square = activeSquares[i];

				if (square.Mask != 0)
				{
					var collider = edgeCollider.AddCollider();

					collider.points = Trace(square, edgeCollider.Optimize);

					cell.Colliders.Add(collider);
				}
			}
		}
	}
}