using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component slices a shape at the collision impact point when another object hits this destructible object.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dFracturer")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Fracturer")]
	public class D2dFracturer : MonoBehaviour
	{
		/// <summary>If you enable this then this destructible sprite will automatically be fractured when the attached D2dDamage.Damage value is high enough.</summary>
		public bool Damage { set { damage = value; } get { return damage; } } [SerializeField] private bool damage;

		/// <summary>The D2dDamage.Damage value must be at or above this value.</summary>
		public float DamageRequired { set { damageRequired = value; } get { return damageRequired; } } [SerializeField] private float damageRequired = 100.0f;

		/// <summary>After a successful fracture, the damageRequired value will be multiplied by this, allowing for multiple fractures.</summary>
		public float DamageMultiplier { set { damageMultiplier = value; } get { return damageMultiplier; } } [SerializeField] private float damageMultiplier = 2.0f;

		/// <summary>This lets you set how many fracture points there can be based on the amount of solid pixels.</summary>
		public float PointsPerSolidPixel { set { pointsPerSolidPixel = value; } get { return pointsPerSolidPixel; } } [SerializeField] private float pointsPerSolidPixel = 0.001f;

		/// <summary>This lets you limit how many points the fracture can use.</summary>
		public int MaxPoints { set { maxPoints = value; } get { return maxPoints; } } [SerializeField] private int maxPoints = 10;

		/// <summary>Automatically multiply the points by the D2dDestructible.AlphaSharpness value to account for optimizations?</summary>
		public bool FactorInSharpness { set { factorInSharpness = value; } get { return factorInSharpness; } } [SerializeField] private bool factorInSharpness = true;

		/// <summary>Fracturing can cause pixel islands to appear, should a split be triggered on each fractured part to check for these?</summary>
		public bool SplitAfterFracture { set { splitAfterFracture = value; } get { return splitAfterFracture; } } [SerializeField] private bool splitAfterFracture;

		/// <summary>This allows you to set the Feather value used when splitting.</summary>
		public int SplitFeather { set { splitFeather = value; } get { return splitFeather; } } [SerializeField] private int splitFeather = 3;

		/// <summary>This allows you to set the HealThreshold value used when splitting.</summary>
		public int SplitHealThreshold { set { splitHealThreshold = value; } get { return splitHealThreshold; } } [SerializeField] private int splitHealThreshold = -1;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[System.NonSerialized]
		private bool cachedDestructibleSet;

		[System.NonSerialized]
		private D2dDamage cachedDamage;

		private static int[] voronoi;

		private static Color32[] tempAlphaData;

		private static int voronoiCount;

		private static List<D2dRect> voronoiRects = new List<D2dRect>();

		private static List<Vector2> voronoiPoints = new List<Vector2>();

		public int FinalPoints
		{
			get
			{
				if (cachedDestructibleSet == false)
				{
					cachedDestructible    = GetComponent<D2dDestructible>();
					cachedDestructibleSet = true;
				}

				return CalculatePointCount(cachedDestructible, pointsPerSolidPixel, factorInSharpness, maxPoints);
			}
		}

		[ContextMenu("Try Fracture")]
		public void Fracture()
		{
			TryFracture();
		}

		public static int CalculatePointCount(D2dDestructible destructible, float pointsPerSolidPixel, bool factorInSharpness, int maxPoints)
		{
			var points = destructible.AlphaCount * pointsPerSolidPixel;

			if (factorInSharpness == true)
			{
				points *= destructible.AlphaSharpness * destructible.AlphaSharpness;
			}

			return Mathf.Min(Mathf.CeilToInt(points), maxPoints);
		}

		public bool TryFracture()
		{
			var points = FinalPoints; // NOTE: This will also set cachedDestructible

			return TryFracture(cachedDestructible, points, splitAfterFracture, splitFeather, splitHealThreshold);
		}

		private static float recip255 = 1.0f / 255.0f;

		private static List<D2dDestructible> chunks = new List<D2dDestructible>();

		/// <summary>This method allows you to manually try and fracture the specified D2dDestructible.</summary>
		public static bool TryFracture(D2dDestructible destructible, int pointCount, bool splitAfterFracture, int splitFeather, int splitHealThreshold)
		{
			if (pointCount > 1 && destructible != null && destructible.Ready == true)
			{
				if (pointCount > 20)
				{
					pointCount = 20;
				}

				var w = destructible.AlphaWidth;
				var h = destructible.AlphaHeight;
				var t = w * h;

				GenerateVoronoiPoints(pointCount, w, h);
				GenerateVoronoiData(pointCount, w, h, t);

				if (voronoiCount > 1)
				{
					var alphaData = destructible.AlphaData;
					var alphaRect = new D2dRect(0, w, 0, h);

					destructible.SplitBegin();

					chunks.Clear();

					for (var i = pointCount - 1; i >= 0; i--)
					{
						var rect = voronoiRects[i];

						if (rect.IsSet == true)
						{
							voronoiCount -= 1;

							var chunk = destructible.SplitNext(voronoiCount == 0);

							chunks.Add(chunk);

							rect.Expand(1);
							rect.ClampTo(alphaRect);

							D2dCommon.ReserveTempAlphaData(rect.SizeX, rect.SizeY);
						
							if (tempAlphaData == null || tempAlphaData.Length < rect.SizeX * rect.SizeY)
							{
								tempAlphaData = new Color32[rect.SizeX * rect.SizeY];
							}

							// Write black and white mask
							for (var y = rect.MinY; y < rect.MaxY; y++)
							{
								var o = y * w;
								var z = (y - rect.MinY) * rect.SizeX - rect.MinX;

								for (var x = rect.MinX; x < rect.MaxX; x++)
								{
									var alpha = voronoi[o + x] == i ? 255 : 0;

									D2dCommon.tempAlphaData[z + x] = new Color32(255, 255, 255, (byte)alpha);
								}
							}

							// Blur
							D2dBlur.BlurHorizontally(D2dCommon.tempAlphaData, tempAlphaData, rect.SizeX, rect.SizeY);
							D2dBlur.BlurVertically(tempAlphaData, D2dCommon.tempAlphaData, rect.SizeX, rect.SizeY);

							// Combine alpha
							for (var y = rect.MinY; y < rect.MaxY; y++)
							{
								var o = y * w;
								var z = (y - rect.MinY) * rect.SizeX - rect.MinX;

								for (var x = rect.MinX; x < rect.MaxX; x++)
								{
									var index      = z + x;
									var maskPixel  = D2dCommon.tempAlphaData[index];
									var alphaPixel = alphaData[o + x];

									alphaPixel.a = (byte)(alphaPixel.a * (maskPixel.a * recip255));

									D2dCommon.tempAlphaData[index] = alphaPixel;
								}
							}

							chunk.SubsetAlphaWith(D2dCommon.tempAlphaData, rect);
						}
					}

					destructible.SplitEnd(D2dDestructible.SplitMode.Fracture);

					if (splitAfterFracture == true)
					{
						for (var i = chunks.Count - 1; i >= 0; i--)
						{
							var chunk = chunks[i];

							if (chunk != null)
							{
								D2dSplitter.TrySplit(chunk, splitFeather, splitHealThreshold);
							}
						}
					}

					return true;
				}
			}

			return false;
		}

		protected virtual void Update()
		{
			if (damage == true)
			{
				if (cachedDamage == null) cachedDamage = GetComponent<D2dDamage>();

				if (cachedDamage != null)
				{
					if (cachedDamage.Damage >= damageRequired)
					{
						// Fracturing will clone the current damageRequired value, so halve in now in case it gets split, and revert if it wasn't split
						var oldDamageRequired = damageRequired;

						damageRequired = cachedDamage.Damage * damageMultiplier;

						if (TryFracture() == false)
						{
							damageRequired = oldDamageRequired;
						}
					}
				}
			}
		}

		private static void GenerateVoronoiPoints(int pointCount, float w, float h)
		{
			voronoiPoints.Clear();
			voronoiRects.Clear();

			for (var i = 0; i < pointCount; i++)
			{
				var x = Random.Range(w * 0.1f, w * 0.9f);
				var y = Random.Range(h * 0.1f, h * 0.9f);

				voronoiPoints.Add(new Vector2(x, y));
				voronoiRects.Add(new D2dRect());
			}
		}

		private static void GenerateVoronoiData(int pointCount, int w, int h, int t)
		{
			if (voronoi == null || voronoi.Length < t)
			{
				voronoi = new int[t];
			}

			for (var y = 0; y < h; y++)
			{
				var o = y * w;
				var v = new Vector2(0.0f, y);

				for (var x = 0; x < w; x++)
				{
					var bestPoint    = 0;
					var bestDistance = float.PositiveInfinity;

					v.x = x;

					for (var i = 0; i < pointCount; i++)
					{
						var distance = Vector2.SqrMagnitude(voronoiPoints[i] - v);

						if (distance < bestDistance)
						{
							bestDistance = distance;
							bestPoint    = i;
						}
					}

					var rect = voronoiRects[bestPoint];

					rect.Add(x, y);

					voronoiRects[bestPoint] = rect;

					voronoi[o + x] = bestPoint;
				}
			}

			voronoiCount = 0;

			for (var i = voronoiRects.Count - 1; i >= 0; i--)
			{
				if (voronoiRects[i].IsSet == true)
				{
					voronoiCount += 1;
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dFracturer;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dFracturer_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Info("Based on these settings, this D2dDestructible will be fractured with " + tgt.FinalPoints + " fracture points.");

			Separator();

			Draw("pointsPerSolidPixel", "This lets you set how many fracture points there can be based on the amount of solid pixels.");
			Draw("maxPoints", "This lets you limit how many points the fracture can use.");
			Draw("factorInSharpness", "Automatically multiply the points by the D2dDestructible.AlphaSharpness value to account for optimizations?");
			Draw("splitAfterFracture", "Fracturing can cause pixel islands to appear, should a split be triggered on each fractured part to check for these?");

			if (Any(tgts, t => t.SplitAfterFracture == true))
			{
				BeginIndent();
					Draw("splitFeather", "This allows you to set the feather value used when splitting.");
					Draw("splitHealThreshold", "This allows you to set the HealThreshold value used when splitting.");
				EndIndent();
			}

			Separator();

			Draw("damage", "If you enable this then this destructible sprite will automatically be fractured when the attached D2dDamage.Damage value is high enough.");
			if (Any(tgts, t => t.Damage == true))
			{
				if (Any(tgts, t => t.GetComponent<D2dDamage>() == null))
				{
					Warning("There is no D2dDamage on this GameObject, so you cannot require Damage.");
				}
				BeginIndent();
					Draw("damageRequired", "The D2dDamage.Damage value must be at least this value.");
					Draw("damageMultiplier", "After a successful fracture, the damageRequired value will be multiplied by this, allowing for multiple fractures.");
				EndIndent();
			}

			Separator();

			if (Button("Try Fracture") == true)
			{
				Each(tgts, t => t.TryFracture());
			}
		}
	}
}
#endif