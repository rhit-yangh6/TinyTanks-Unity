using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to generate polygon colliders for a destructible sprite. Polygon colliders should be used for moving objects.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dPolygonCollider")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Polygon Collider")]
	public class D2dPolygonCollider : D2dCollider
	{
		public enum CellSizes
		{
			Square8  = 8,
			Square16 = 16,
			Square32 = 32,
			Square64 = 64
		}

		[System.Serializable]
		public class Path : IComparer<Path>
		{
			public Vector2[] Points;
			public Vector2   Left;

			public void CalculateLeft()
			{
				Left.x = float.PositiveInfinity;

				for (var i = Points.Length - 1; i >= 0; i--)
				{
					var point = Points[i];

					if (point.x < Left.x)
					{
						Left = point;
					}
				}
			}

			public static float LineSide(Vector2 a, Vector2 b, Vector2 p)
			{
				return (b.y - a.y) * (p.x - a.x) - (b.x - a.x) * (p.y - a.y);
			}

			public bool Contains(Vector2 point)
			{
				var total  = 0;
				var pointA = Points[0];

				for (var j = Points.Length - 1; j >= 0; j--)
				{
					var pointB = Points[j];

					if (pointA.y <= point.y)
					{
						if (pointB.y > point.y && LineSide(pointA, pointB, point) > 0.0f) total += 1;
					}
					else
					{
						if (pointB.y <= point.y && LineSide(pointA, pointB, point) < 0.0f) total -= 1;
					}

					pointA = pointB;
				}

				return total != 0;
			}

			public int Compare(Path a, Path b)
			{
				return a.Left.x.CompareTo(b.Left.x);
			}
		}

		[System.Serializable]
		public class Shape
		{
			public PolygonCollider2D Collider;

			public Path Outside;

			public List<Path> Holes = new List<Path>();

			public bool Contains(Vector2 point)
			{
				if (Outside.Contains(point) == true)
				{
					for (var i = 0; i < Holes.Count; i++)
					{
						if (Holes[i].Contains(point) == true)
						{
							return false;
						}
					}

					return true;
				}

				return false;
			}
		}

		[System.Serializable]
		public class Cell
		{
			public List<Shape> Shapes = new List<Shape>();
		}

		/// <summary>This allows you to change the pixel width & height of each collider cell to improve performance. The pixel size you choose should be in relation to the typical size of destruction in your scene.</summary>
		public CellSizes CellSize { set { cellSize = value; Rebuild(); } get { return cellSize; } } [SerializeField] private CellSizes cellSize = CellSizes.Square16;

		/// <summary>This allows you to set the <b>CellSize</b> value without causing <b>Rebuild</b> to be called.</summary>
		public void SetCellSize(CellSizes value) { cellSize = value; }

		/// <summary>This allows you to control how easily the edges can merge together. A higher value gives better performance, but less accurate colliders.</summary>
		public float Straighten { set { straighten = value; Rebuild(); } get { return straighten; } } [SerializeField] [Range(0.0f, 0.5f)] private float straighten = 0.01f;

		/// <summary>This allows you to set the <b>Straighten</b> value without causing <b>Rebuild</b> to be called.</summary>
		public void SetStraighten(float value) { straighten = value; }

		[SerializeField]
		private int cellRow;

		[SerializeField]
		private int cellCol;

		[SerializeField]
		private int cellSiz;

		[SerializeField]
		private List<Cell> cells;

		[System.NonSerialized]
		private static Stack<PolygonCollider2D> colliderPool = new Stack<PolygonCollider2D>();

		[System.NonSerialized]
		private static Stack<Cell> cellPool = new Stack<Cell>();

		[System.NonSerialized]
		private static Stack<Shape> shapePool = new Stack<Shape>();

		[ContextMenu("Refresh")]
		public override void Refresh()
		{
			for (var i = cells.Count - 1; i >= 0; i--)
			{
				var cell = cells[i];

				for (var j = cell.Shapes.Count - 1; j >= 0; j--)
				{
					var shape = cell.Shapes[j];

					if (shape.Collider != null)
					{
						Refresh(shape.Collider);
					}
				}
			}
		}

		public PolygonCollider2D AddCollider()
		{
			var collider = colliderPool.Count > 0 ? colliderPool.Pop() : child.AddComponent<PolygonCollider2D>();

			Refresh(collider);

			return collider;
		}

		private void Refresh(PolygonCollider2D collider)
		{
			collider.sharedMaterial  = material;
			collider.isTrigger       = isTrigger;

			if (collider.usedByEffector != usedByEffector)
			{
				collider.usedByEffector = usedByEffector;
			}

			if (collider.usedByComposite != usedByComposite)
			{
				collider.usedByComposite = usedByComposite;
			}

			if (UseAutoMass == true)
			{
				collider.density = density;
			}
		}

		protected override void DoModified(D2dRect rect)
		{
			var cellXMin = rect.MinX / cellSiz;
			var cellYMin = rect.MinY / cellSiz;
			var cellXMax = (rect.MaxX + 1) / cellSiz;
			var cellYMax = (rect.MaxY + 1) / cellSiz;

			cellXMin = Mathf.Clamp(cellXMin, 0, cellCol - 1);
			cellXMax = Mathf.Clamp(cellXMax, 0, cellCol - 1);
			cellYMin = Mathf.Clamp(cellYMin, 0, cellRow - 1);
			cellYMax = Mathf.Clamp(cellYMax, 0, cellRow - 1);

			for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
			{
				var offset = cellY * cellCol;

				for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
				{
					ClearCell(cells[cellX + offset]);
				}
			}

			for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
			{
				var offset = cellY * cellCol;

				for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
				{
					var cell = cells[cellX + offset];

					RebuildCell(cell, cellX, cellY);
				}
			}

			SweepColliders();
		}

		protected override void HandleSplitStart()
		{
			base.HandleSplitStart();

			ClearCells();
			SweepColliders();
		}

		protected override void HandleSplitEnd(List<D2dDestructible> splitDestructibles, D2dDestructible.SplitMode mode)
		{
			base.HandleSplitEnd(splitDestructibles, mode);

			for (var i = splitDestructibles.Count - 1; i >= 0; i--)
			{
				var splitDestructible = splitDestructibles[i];
				var polygonCollider   = splitDestructible.GetComponent<D2dPolygonCollider>();

				if (polygonCollider != null)
				{
					polygonCollider.Rebuild();
				}
			}
		}

		protected override void DoRebuild()
		{
			if (cells == null)
			{
				cells = new List<Cell>();
			}

			ClearCells();

			var sprite = CachedDestructible;

			cellSiz = (int)cellSize;
			cellCol = (sprite.AlphaWidth  + cellSiz - 1) / cellSiz;
			cellRow = (sprite.AlphaHeight + cellSiz - 1) / cellSiz;

			for (var cellY = 0; cellY < cellRow; cellY++)
			{
				for (var cellX = 0; cellX < cellCol; cellX++)
				{
					var cell = cellPool.Count > 0 ? cellPool.Pop() : new Cell();

					cells.Add(cell);

					RebuildCell(cell, cellX, cellY);
				}
			}

			SweepColliders();
		}

		private void ClearCells()
		{
			for (var i = cells.Count - 1; i >= 0; i--)
			{
				var cell = cells[i];
				
				if (cell != null)
				{
					ClearCell(cell);

					cellPool.Push(cell);
				}
			}

			cells.Clear();
		}

		private void SweepColliders()
		{
			while (colliderPool.Count > 0)
			{
				CwHelper.Destroy(colliderPool.Pop());
			}
		}

		private void RebuildCell(Cell cell, int cellX, int cellY)
		{
			var x    = cellSiz * cellX;
			var y    = cellSiz * cellY;
			var xMin = x - 1;
			var yMin = y - 1;
			var xMax = Mathf.Min(x + cellSiz, CachedDestructible.AlphaWidth );
			var yMax = Mathf.Min(y + cellSiz, cachedDestructible.AlphaHeight);
			
			D2dPolygonSquares.AlphaD = cachedDestructible.AlphaData;
			D2dPolygonSquares.AlphaW = cachedDestructible.AlphaWidth;
			D2dPolygonSquares.AlphaH = cachedDestructible.AlphaHeight;
			D2dPolygonSquares.MinX   = xMin;
			D2dPolygonSquares.MinY   = yMin;
			D2dPolygonSquares.MaxX   = xMax;
			D2dPolygonSquares.MaxY   = yMax;

			D2dPolygonSquares.CalculateCells();
			D2dPolygonSquares.Build(cell, shapePool, this);
		}

		private void ClearCell(Cell cell)
		{
			if (cell.Shapes != null)
			{
				for (var j = cell.Shapes.Count - 1; j >= 0; j--)
				{
					var ring = cell.Shapes[j];

					if (ring.Collider != null)
					{
						ring.Collider.pathCount = 0;

						colliderPool.Push(ring.Collider);
					}

					shapePool.Push(ring);
				}

				cell.Shapes.Clear();
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dPolygonCollider;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dPolygonCollider_Editor : D2dCollider_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			base.OnInspector();

			var rebuild = false;

			Draw("cellSize", ref rebuild, "This allows you to change the pixel width & height of each collider cell to improve performance. The pixel size you choose should be in relation to the typical size of destruction in your scene.");
			Draw("straighten", ref rebuild, "This allows you to control how easily the edges can merge together. A higher value gives better performance, but less accurate colliders.");

			Separator();

			if (Button("Rebuild") == true)
			{
				Each(tgts, t => t.Rebuild(), true);
			}

			if (rebuild == true) Each(tgts, t => t.Rebuild(), true);
		}
	}
}
#endif