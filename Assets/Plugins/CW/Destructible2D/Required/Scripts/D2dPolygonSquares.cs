using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	/// <summary>This allows you to calculate the polygons of a specific alphaData area.</summary>
	public static class D2dPolygonSquares
	{
		enum Side
		{
			Left,
			Right,
			Bottom,
			Top
		}

		class Square
		{
			public int   Mask;
			public int   Index;
			public Point PointL;
			public Point PointR;
			public Point PointB;
			public Point PointT;
		}

		struct Point
		{
			public int  x;
			public int  y;
			public bool e;

			public static Point operator - (Point a, Point b) { a.x -= b.x; a.y -= b.y; return a; }
			public static implicit operator Vector2(Point a) { return new Vector2(a.x, a.y); }
		}

		private static List<Square> squares = new List<Square>();

		private static List<Square> activeSquares = new List<Square>();

		private static Square square;
		private static Side   squareSide;
		private static int    squareIndex;
		private static int    squareCount;
		private static int    squareWidth;
		private static int    squareHeight;
		private static int    squareMinX;
		private static int    squareMinY;
		private static int    squareMaxX;
		private static int    squareMaxY;

		public static Color32[] AlphaD; // Raw data
		public static int       AlphaW; // Width
		public static int       AlphaH; // Height
		public static int       MinX;
		public static int       MaxX;
		public static int       MinY;
		public static int       MaxY;

		private static List<Point> points = new List<Point>();

		private static List<Vector2> weldedPoints = new List<Vector2>();

		private static List<D2dPolygonCollider.Path> paths = new List<D2dPolygonCollider.Path>();

		private static int pathCount;

		private static byte GetPolyAlpha(int x, int y)
		{
			if (x >= 0 && x >= MinX && x < MaxX)
			{
				if (y >= 0 && y >= MinY && y < MaxY)
				{
					return AlphaD[x + y * AlphaW].a;
				}
			}

			return 0;
		}

		private static int[,] subSquares = new int[256, 256];

		static D2dPolygonSquares()
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
			squareWidth  = MaxX - MinX + 2;
			squareHeight = MaxY - MinY + 2;
			squareMinX   = MinX * 255;
			squareMaxX   = MaxX * 255 - 255;
			squareMinY   = MinY * 255;
			squareMaxY   = MaxY * 255 - 255;

			var reserve = squareWidth * squareHeight - squares.Count; for (var i = reserve; i > 0; i--) squares.Add(new Square());

			activeSquares.Clear();

			for (var y = MinY - 1; y <= MaxY; y++)
			{
				var o  = (y - MinY + 1) * squareWidth - MinX + 1;
				var bl = GetPolyAlpha(0    , y    ); var useBl = bl >= 128;
				var tl = GetPolyAlpha(0    , y + 1); var useTl = tl >= 128;

				for (var x = MinX - 1; x <= MaxX; x++)
				{
					var square = squares[x + o];
					var br     = GetPolyAlpha(x + 1, y    ); var useBr = br >= 128;
					var tr     = GetPolyAlpha(x + 1, y + 1); var useTr = tr >= 128;
					var mask   = ((useBl ? 1 : 0) + (useBr ? 2 : 0) + (useTl ? 4 : 0) + (useTr ? 8 : 0)) % 15;

					if (mask > 0)
					{
						var xp = x * 255;
						var yp = y * 255;

						square.Mask  = mask;
						square.Index = x + o;

						activeSquares.Add(square);

						if (useBl ^ useBr) square.PointB = ClampPoint(xp + subSquares[bl, br], yp      );
						if (useTl ^ useTr) square.PointT = ClampPoint(xp + subSquares[tl, tr], yp + 255);
						if (useBl ^ useTl) square.PointL = ClampPoint(xp      , yp + subSquares[bl, tl]);
						if (useBr ^ useTr) square.PointR = ClampPoint(xp + 255, yp + subSquares[br, tr]);
					}

					bl = br; useBl = useBr;
					tl = tr; useTl = useTr;
				}
			}
		}

		private static Point ClampPoint(int x, int y)
		{
			var e = false;

			if (x < squareMinX) { x = squareMinX; e = true; } else if (x > squareMaxX) { x = squareMaxX; e = true; }
			if (y < squareMinY) { y = squareMinY; e = true; } else if (y > squareMaxY) { y = squareMaxY; e = true; }

			return new Point{x = x, y = y, e = e};
		}

		private static Vector2[] Trace(float straighten)
		{
			while (true)
			{
				switch (square.Mask)
				{
					case 0:
					{
						weldedPoints.Clear();

						var head      = points[0];
						var delta     = default(Point);
						var direction = default(Vector2);
						var edge      = false;

						weldedPoints.Add(head);

						straighten = 1.0f - straighten;

						for (var i = points.Count - 1; i >= 1; i--)
						{
							var point = points[i];

							if (point.x != head.x || point.y != head.y)
							{
								var newDelta     = point - head;
								var newDirection = ((Vector2)newDelta).normalized;
								var different    = newDelta.x != delta.x || newDelta.y != delta.y;
								var newEdge      = point.e;

								if (different == true && (newEdge != edge || Vector2.Dot(direction, newDirection) < straighten))
								{
									delta     = newDelta;
									direction = newDirection;

									weldedPoints.Add(point);
								}
								else
								{
									weldedPoints[weldedPoints.Count - 1] = point;
								}

								edge = newEdge;
								head = point;
							}
						}
					}
					return D2dCache.CachedList<Vector2>.ToArray(weldedPoints);

					case 1: square.Mask = 0; SubmitL(); break;

					case 2: square.Mask = 0; SubmitB(); break;

					case 3: square.Mask = 0; SubmitL(); break;

					case 4: square.Mask = 0; SubmitT(); break;

					case 5: square.Mask = 0; SubmitT(); break;

					case 6:
					{
						if (squareSide == Side.Right)
						{
							square.Mask = 14; SubmitT();
						}
						else
						{
							square.Mask = 7; SubmitB();
						}
					}
					break;

					case 7: square.Mask = 0; SubmitT(); break;

					case 8: square.Mask = 0; SubmitR(); break;

					case 9:
					{
						if (squareSide == Side.Top)
						{
							square.Mask = 13; SubmitL();
						}
						else
						{
							square.Mask = 11; SubmitR();
						}
					}
					break;

					case 10: square.Mask = 0; SubmitB(); break;

					case 11: square.Mask = 0; SubmitL(); break;

					case 12: square.Mask = 0; SubmitR(); break;

					case 13: square.Mask = 0; SubmitR(); break;

					case 14: square.Mask = 0; SubmitB(); break;
				}

				square = squares[squareIndex];
			}
		}

		private static void SubmitL()
		{
			points.Add(square.PointL); squareIndex -= 1; squareSide = Side.Right;
		}

		private static void SubmitR()
		{
			points.Add(square.PointR); squareIndex += 1; squareSide = Side.Left;
		}

		private static void SubmitB()
		{
			points.Add(square.PointB); squareIndex -= squareWidth; squareSide = Side.Top;
		}

		private static void SubmitT()
		{
			points.Add(square.PointT); squareIndex += squareWidth; squareSide = Side.Bottom;
		}

		public static void Build(D2dPolygonCollider.Cell cell, Stack<D2dPolygonCollider.Shape> shapePool, D2dPolygonCollider parent)
		{
			pathCount = 0;

			D2dCache.CachedList<Vector2>.Clear();

			for (var i = 0; i < activeSquares.Count; i++)
			{
				square = activeSquares[i];

				if (square.Mask != 0)
				{
					squareIndex = square.Index;

					points.Clear();

					var newPoints = Trace(parent.Straighten);

					if (newPoints != null && newPoints.Length > 2)
					{
						var path = GetNextPath();

						path.Points = newPoints;
					}
				}
			}

			if (pathCount > 0)
			{
				SortPaths();
			}

			for (var i = 0; i < pathCount; i++)
			{
				var path       = paths[i];
				var shapeIndex = FindShapeIndex(cell, path.Left, i);

				// Combine?
				if (shapeIndex >= 0)
				{
					var shape = cell.Shapes[shapeIndex];

					shape.Holes.Add(path);

					shape.Collider.pathCount += 1;

					shape.Collider.SetPath(shape.Collider.pathCount - 1, path.Points);
				}
				// New?
				else
				{
					var shape = shapePool.Count > 0 ? shapePool.Pop() : new D2dPolygonCollider.Shape();

					shape.Outside = path;
					shape.Holes.Clear();

					shape.Collider = parent.AddCollider();

					shape.Collider.SetPath(0, path.Points);

					cell.Shapes.Add(shape);
				}
			}
		}

		private static int FindShapeIndex(D2dPolygonCollider.Cell cell, Vector2 point, int max)
		{
			for (var i = 0; i < cell.Shapes.Count; i++)
			{
				var shape = cell.Shapes[i];

				if (shape.Contains(point) == true)
				{
					return i;
				}
			}

			return -1;
		}

		private static void SortPaths()
		{
			for (var i = 0; i < pathCount; i++)
			{
				paths[i].CalculateLeft();
			}

			paths.Sort(0, pathCount, paths[0]);
		}

		private static D2dPolygonCollider.Path GetNextPath()
		{
			var path = default(D2dPolygonCollider.Path);

			if (pathCount >= paths.Count)
			{
				path = new D2dPolygonCollider.Path(); paths.Add(path);
			}
			else
			{
				path = paths[pathCount];
			}

			pathCount += 1;

			return path;
		}
	}
}