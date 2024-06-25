using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to split the attached destructible sprite into multiple pieces if you slice it in half, or otherwise damage it in a way that leaves multiple separated pixel 'islands'.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dSplitter")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Splitter")]
	public class D2dSplitter : MonoBehaviour
	{
		public struct Pixel
		{
			public int i;
			public int x;
			public int y;
		}

		public class Line
		{
			public bool   Used;
			public int    Y;
			public int    MinX;
			public int    MaxX;
			public Island Island;

			public List<Line> Ups = new List<Line>();
			public List<Line> Dns = new List<Line>();
		}

		public class Island
		{
			public int MinX;
			public int MinY;
			public int MaxX;
			public int MaxY;
			public int Count;

			public List<Line> Lines = new List<Line>();
			public List<Pixel> Pixels = new List<Pixel>();

			private static D2dDistanceField distanceField = new D2dDistanceField();

			public void Clear()
			{
				Lines.Clear();
				Pixels.Clear();
			}

			public void Fill(D2dDistanceField baseField, D2dRect baseRect, D2dRect rect)
			{
				distanceField.Transform(rect, this);

				for (var y = rect.MinY; y < rect.MaxY; y++)
				{
					var o = (y - rect.MinY) * rect.SizeX - rect.MinX;
					var z = y * alphaWidth;

					for (var x = rect.MinX; x < rect.MaxX; x++)
					{
						var cell     = distanceField.Cells[x - rect.MinX + (y - rect.MinY) * rect.SizeX];
						var baseCell = baseField.Cells[x - baseRect.MinX + (y - baseRect.MinY) * baseRect.SizeX];

						if (cell.d == baseCell.d)
						{
							D2dCommon.tempAlphaData[o + x] = alphaData[z + x];
						}
					}
				}
			}

			public void Fill(D2dRect rect)
			{
				for (var i = Lines.Count - 1; i >= 0; i--)
				{
					var line = Lines[i];
					var o    = (line.Y - rect.MinY) * rect.SizeX - rect.MinX;
					var z    = line.Y * alphaWidth;

					for (var x = line.MinX; x < line.MaxX; x++)
					{
						D2dCommon.tempAlphaData[o + x] = alphaData[z + x];
					}
				}
			}
		}

		/// <summary>If your destructible sprite has soft edges then you should increase the feather distance to the pixel thickness of your soft edges.</summary>
		public int Feather { set { feather = value; } get { return feather; } } [SerializeField] private int feather = 3;

		/// <summary>Split pieces cannot be healed by default, this is because healing split pieces can often lead to unexpected behavior and worse performance. If you really want to allow healing of split pieces, then this setting allows you to control how many pixels must remain in the split piece for it to be eligible for healing.
		/// -1 = Split pieces cannot be healed.
		/// 0 = All split pieces can be healed.
		/// 10 = Split pieces must have more than 10 AlphaCount to be healed.</summary>
		public int HealThreshold { set { healThreshold = value; } get { return healThreshold; } } [SerializeField] private int healThreshold = -1;

		private static List<Line> lines = new List<Line>();

		private static List<Line> linkedLines = new List<Line>();

		private static int lineCount;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[System.NonSerialized]
		private bool cachedDestructibleSet;

		public D2dDestructible CachedDestructible
		{
			get
			{
				if (cachedDestructibleSet == false)
				{
					cachedDestructible    = GetComponent<D2dDestructible>();
					cachedDestructibleSet = true;
				}

				return cachedDestructible;
			}
		}

		protected virtual void OnEnable()
		{
			if (cachedDestructibleSet == false)
			{
				cachedDestructible    = GetComponent<D2dDestructible>();
				cachedDestructibleSet = true;
			}

			cachedDestructible.OnRebuilt  += Rebuild;
			cachedDestructible.OnModified += Modified;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnRebuilt  -= Rebuild;
			cachedDestructible.OnModified -= Modified;
		}

		private static Color32[] alphaData;
		private static bool[] alphaRemaining;
		private static int alphaWidth;
		private static int alphaHeight;
		private static List<Island> islands = new List<Island>();
		private static Stack<Island> islandPool = new Stack<Island>();
		private static D2dDistanceField baseField = new D2dDistanceField();

		[ContextMenu("Split")]
		public void TrySplit()
		{
			TrySplit(CachedDestructible, feather, healThreshold);
		}

		public static void TrySplit(D2dDestructible destructible, int feather, int healThreshold)
		{
			if (destructible != null)
			{
				Search(destructible);

				if (islands.Count > 1)
				{
					var baseRect = new D2dRect(0, alphaWidth, 0, alphaHeight);

					if (feather > 0)
					{
						baseField.Transform(baseRect, alphaWidth, alphaHeight, alphaData);

						destructible.SplitBegin();

						for (var i = islands.Count - 1; i >= 0; i--)
						{
							var island = islands[i];
							var piece  = destructible.SplitNext(i == 0);
							var rect   = default(D2dRect);

							if (healThreshold >= 0 && island.Count >= healThreshold)
							{
								rect = baseRect;
							}
							else
							{
								rect.MinX = island.MinX;
								rect.MaxX = island.MaxX;
								rect.MinY = island.MinY;
								rect.MaxY = island.MaxY;

								rect.Expand(feather);
								
								rect.ClampTo(baseRect);

								piece.HealSnapshot = null;
							}

							D2dCommon.ReserveTempAlphaDataClear(rect.SizeX, rect.SizeY);

							island.Fill(baseField, baseRect, rect);

							piece.SubsetAlphaWith(D2dCommon.tempAlphaData, rect, island.Count);
						}
					}
					else
					{
						destructible.SplitBegin();

						for (var i = islands.Count - 1; i >= 0; i--)
						{
							var island = islands[i];
							var chunk  = destructible.SplitNext(i == 0);
							var rect   = new D2dRect(island.MinX, island.MaxX, island.MinY, island.MaxY); rect.ClampTo(baseRect);

							D2dCommon.ReserveTempAlphaDataClear(rect.SizeX, rect.SizeY);

							island.Fill(rect);

							chunk.SubsetAlphaWith(D2dCommon.tempAlphaData, rect);
						}
					}

					destructible.SplitEnd(D2dDestructible.SplitMode.Split);
				}
			}
		}

		public static void Clear()
		{
			Clear(islands);
		}

		private static void Clear(List<Island> islands)
		{
			for (var i = islands.Count - 1; i >= 0; i--)
			{
				var island = islands[i];

				island.Clear();

				islandPool.Push(island);
			}

			islands.Clear();
		}

		public static void Search(D2dDestructible destructible)
		{
			islands.Clear();

			if (destructible == null || destructible.Ready == false)
			{
				return;
			}

			alphaData   = destructible.AlphaData;
			alphaWidth  = destructible.AlphaWidth;
			alphaHeight = destructible.AlphaHeight;

			var rect = new D2dRect(0, alphaWidth, 0, alphaHeight);

			if (rect.MinX < 0) rect.MinX = 0;
			if (rect.MinY < 0) rect.MinY = 0;
			if (rect.MaxX > alphaWidth ) rect.MaxX = alphaWidth;
			if (rect.MaxY > alphaHeight) rect.MaxY = alphaHeight;

			lineCount = 0;

			var oldCount = 0;

			for (var y = rect.MinY; y < rect.MaxY; y++)
			{
				var newCount = FastFindLines(alphaData, alphaWidth, y, rect.MinX, rect.MaxX);

				FastLinkLines(lineCount - newCount - oldCount, lineCount - newCount, lineCount);

				oldCount = newCount;
			}

			for (var i = 0; i < lineCount; i++)
			{
				var line = lines[i];

				if (line.Used == false)
				{
					var island = islandPool.Count > 0 ? islandPool.Pop() : new Island();

					island.MinX  = line.MinX;
					island.MaxX  = line.MaxX;
					island.MinY  = line.Y;
					island.MaxY  = line.Y + 1;
					island.Count = 0;

					// Scan though all connected lines and add to list
					linkedLines.Clear(); linkedLines.Add(line); line.Used = true;

					for (var j = 0; j < linkedLines.Count; j++)
					{
						var linkedLine = linkedLines[j];

						island.MinX   = Mathf.Min(island.MinX, linkedLine.MinX);
						island.MaxX   = Mathf.Max(island.MaxX, linkedLine.MaxX);
						island.MinY   = Mathf.Min(island.MinY, linkedLine.Y    );
						island.MaxY   = Mathf.Max(island.MaxY, linkedLine.Y + 1);
						island.Count += linkedLine.MaxX - linkedLine.MinX;

						AddToScan(linkedLine.Ups);
						AddToScan(linkedLine.Dns);

						linkedLine.Island = island;

						island.Lines.Add(linkedLine);
					}

					islands.Add(island);
				}
			}
		}

		private static void AddToScan(List<Line> lines)
		{
			for (var i = lines.Count - 1; i >= 0; i--)
			{
				var line = lines[i];

				if (line.Used == false)
				{
					linkedLines.Add(line); line.Used = true;
				}
			}
		}

		private static void FastLinkLines(int min, int mid, int max)
		{
			for (var i = min; i < mid; i++)
			{
				var oldLine = lines[i];

				for (var j = mid; j < max; j++)
				{
					var newLine = lines[j];

					if (newLine.MinX < oldLine.MaxX && newLine.MaxX > oldLine.MinX)
					{
						oldLine.Ups.Add(newLine);
						newLine.Dns.Add(oldLine);
					}
				}
			}
		}

		private static int FastFindLines(Color32[] alphaData, int alphaWidth, int y, int minX, int maxX)
		{
			var line   = default(Line);
			var count  = 0;
			var offset = alphaWidth * y;

			for (var x = minX; x < maxX; x++)
			{
				if (alphaData[offset + x].a >= 128)
				{
					// Start new line?
					if (line == null)
					{
						line = GetLine(); count += 1;
						line.MinX = line.MaxX = x;
						line.Y = y;
					}

					// Expand line
					line.MaxX += 1;
				}
				// Terminate line?
				else if (line != null)
				{
					line = null;
				}
			}

			return count;
		}

		private static Line GetLine()
		{
			var line = default(Line);

			if (lineCount >= lines.Count)
			{
				line = new Line();

				lines.Add(line);
			}
			else
			{
				line = lines[lineCount];

				line.Used = false;
				line.Ups.Clear();
				line.Dns.Clear();
			}

			lineCount += 1;

			return line;
		}

		public void Rebuild()
		{
			//Search();
		}

		private void Modified(D2dRect rect)
		{
			TrySplit();
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dSplitter;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dSplitter_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Feather < 0));
				Draw("feather", "If your destructible sprite has soft edges then you should increase the feather distance to the pixel thickness of your soft edges.");
			EndError();
			Draw("healThreshold", "Split pieces cannot be healed by default, this is because healing split pieces can often lead to unexpected behavior and worse performance. If you really want to allow healing of split pieces, then this setting allows you to control how many pixels must remain in the split piece for it to be eligible for healing.\n\n-1 = Split pieces cannot be healed.\n0 = All split pieces can be healed.\n10 = Split pieces must have more than 10 AlphaCount to be healed.");

			Separator();

			if (Button("Try Split") == true)
			{
				Each(tgts, t => { t.TrySplit(); } );
			}
		}
	}
}
#endif