using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to generate edge colliders for a destructible sprite. Edge colliders should only be used for non-moving objects.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dEdgeCollider")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Edge Collider")]
	public class D2dEdgeCollider : D2dCollider
	{
		public enum CellSizes
		{
			Square8  = 8,
			Square16 = 16,
			Square32 = 32,
			Square64 = 64
		}

		[System.Serializable]
		public class Cell
		{
			public List<EdgeCollider2D> Colliders = new List<EdgeCollider2D>();

			public void Clear()
			{
				for (var i = Colliders.Count - 1; i >= 0; i--)
				{
					var collider = Colliders[i];

					if (collider != null)
					{
						colliderPool.Push(collider);
					}
				}

				Colliders.Clear();
			}
		}

		/// <summary>This allows you to set the <b>edgeRadius</b> setting on each generated collider.</summary>
		public float EdgeRadius { set { edgeRadius = value; Refresh(); } get { return edgeRadius; } } [SerializeField] protected float edgeRadius;

		/// <summary>This allows you to set the <b>EdgeRadius</b> value without causing <b>Refresh</b> to be called.</summary>
		public void SetEdgeRadius(float value) { edgeRadius = value; }

		/// <summary>This allows you to change the pixel width & height of each collider cell to improve performance. The pixel size you choose should be in relation to the typical size of destruction in your scene.</summary>
		public CellSizes CellSize { set { cellSize = value; Rebuild(); } get { return cellSize; } } [SerializeField] private CellSizes cellSize = CellSizes.Square16;

		/// <summary>This allows you to set the <b>CellSize</b> value without causing <b>Rebuild</b> to be called.</summary>
		public void SetCellSize(CellSizes value) { cellSize = value; }

		/// <summary>This allows you to control how easily the edges can merge together. A higher value gives better performance, but less accurate colliders.</summary>
		public float Optimize { set { optimize = value; Rebuild(); } get { return optimize; } } [SerializeField] [Range(0.0f, 0.5f)] private float optimize = 0.01f;

		/// <summary>This allows you to set the <b>Optimize</b> value without causing <b>Rebuild</b> to be called.</summary>
		public void SetOptimize(float value) { optimize = value; }

		[SerializeField]
		private int cellRow;

		[SerializeField]
		private int cellCol;

		[SerializeField]
		private int cellSiz;

		[SerializeField]
		private List<Cell> cells;

		[System.NonSerialized]
		private static Stack<EdgeCollider2D> colliderPool = new Stack<EdgeCollider2D>();

		[System.NonSerialized]
		private static Stack<Cell> cellPool = new Stack<Cell>();

		[ContextMenu("Refresh")]
		public override void Refresh()
		{
			for (var i = cells.Count - 1; i >= 0; i--)
			{
				var cell = cells[i];

				for (var j = cell.Colliders.Count - 1; j >= 0; j--)
				{
					var collider = cell.Colliders[j];

					if (collider != null)
					{
						Refresh(collider);
					}
				}
			}
		}

		public EdgeCollider2D AddCollider()
		{
			var collider = colliderPool.Count > 0 ? colliderPool.Pop() : child.AddComponent<EdgeCollider2D>();

			Refresh(collider);

			return collider;
		}

		private void Refresh(EdgeCollider2D collider)
		{
			collider.sharedMaterial  = material;
			collider.isTrigger       = isTrigger;
			collider.edgeRadius      = edgeRadius;

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
					cells[cellX + offset].Clear();
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
				var edgeCollider      = splitDestructible.GetComponent<D2dEdgeCollider>();

				if (edgeCollider != null)
				{
					edgeCollider.Rebuild();
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
			cellCol = (sprite.AlphaWidth + cellSiz - 1) / cellSiz;
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
					cell.Clear();

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
			var xMin = Mathf.Max(x - 1, 0);
			var yMin = Mathf.Max(y - 1, 0);
			var xMax = Mathf.Min(x + cellSiz, cachedDestructible.AlphaWidth );
			var yMax = Mathf.Min(y + cellSiz, cachedDestructible.AlphaHeight);
			
			D2dEdgeSquares.AlphaD = cachedDestructible.AlphaData;
			D2dEdgeSquares.AlphaW = cachedDestructible.AlphaWidth;
			D2dEdgeSquares.AlphaH = cachedDestructible.AlphaHeight;
			D2dEdgeSquares.MinX   = xMin;
			D2dEdgeSquares.MinY   = yMin;
			D2dEdgeSquares.MaxX   = xMax;
			D2dEdgeSquares.MaxY   = yMax;

			D2dEdgeSquares.CalculateCells();
			D2dEdgeSquares.Build(cell, this);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dEdgeCollider;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dEdgeCollider_Editor : D2dCollider_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			base.OnInspector();

			var refresh = false;
			var rebuild = false;

			Draw("edgeRadius", ref refresh, "This allows you to set the edgeRadius setting on each generated collider.");
			Draw("cellSize", ref rebuild, "This allows you to change the pixel width & height of each collider cell to improve performance. The pixel size you choose should be in relation to the typical size of destruction in your scene.");
			Draw("optimize", ref rebuild, "This allows you to control how easily the edges can merge together. A higher value gives better performance, but less accurate colliders.");

			Separator();

			if (Button("Rebuild") == true)
			{
				Each(tgts, t => t.Rebuild(), true, true);
			}

			if (refresh == true) Each(tgts, t => t.Refresh(), true, true);
			if (rebuild == true) Each(tgts, t => t.Rebuild(), true, true);
		}
	}
}
#endif