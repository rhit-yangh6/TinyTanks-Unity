using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This is the base class for all destructible objects.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dDestructible")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Destructible")]
	public abstract partial class D2dDestructible : D2dLinkedBehaviour<D2dDestructible>
	{
		public enum PixelsType
		{
			Smooth,
			Pixelated,
			PixelatedBinary
		}

		public enum SplitMode
		{
			All,
			Split,
			Fracture
		}

		/// <summary>This is invoked when the whole destruction state changes.</summary>
		public event System.Action OnRebuilt;

		/// <summary>This is invoked when a subset of the destruction state changes.</summary>
		public event System.Action<D2dRect> OnModified;

		/// <summary>This is invoked before the destructible is about to be split into separate parts.</summary>
		public event System.Action OnSplitStart;

		/// <summary>This is invoked after the destructible is split into separate parts, with a list of all the parts.
		/// Last Element = This Destructible.</summary>
		public event System.Action<List<D2dDestructible>, SplitMode> OnSplitEnd;

		/// <summary>This event is invoked after the destructible is modified, and contains indices of all pixels that crossed the 128 opacity boundary.
		/// NOTE: This requires the <b>MonitorPixels</b> setting to be enabled.</summary>
		public event System.Action<List<int>> OnModifiedPixels;

		/// <summary>This event works like <b>OnModifiedPixels</b>, but is statically invoked with a reference to the <b>D2dDestructible</b> whose pixels were modified.</summary>
		public static event System.Action<D2dDestructible, List<int>> OnGlobalModifiedPixels;

		/// <summary>If you want to be able to heal this destructible sprite, then set a snapshot of the healed state here.</summary>
		public D2dSnapshot HealSnapshot { set { healSnapshot = value; } get { return healSnapshot; } } [SerializeField] private D2dSnapshot healSnapshot;

		/// <summary>This allows you to manually control the sharpness of the alpha gradient (0 = AlphaSharpness) (+ = OverrideSharpness) (- = AlphaSharpness * -OverrideSharpness).</summary>
		public float OverrideSharpness { set { overrideSharpness = value; } get { return overrideSharpness; } } [SerializeField] private float overrideSharpness;

		/// <summary>This allows you to control how easily this object can be painted.
		/// 1.0 = Default.
		/// 2.0 = Twice as much damage.
		/// 0.5 = Half as much damage.</summary>
		public float PaintMultiplier { set { paintMultiplier = value; } get { return paintMultiplier; } } [SerializeField] private float paintMultiplier = 1.0f;

		/// <summary>This allows you to make it so some pixels are harder to destroy than others, based on their alpha value (0 .. 255).
		/// 0 = Every pixel is equally easy to destroy.
		/// 1 = Values between 0 and 254 receive normal damage, but 255 receives no damage.
		/// 10 = Values between 0 and 245 receive normal damage, but 246 receives 90% damage, 247 receives 80%, etc.</summary>
		public int SolidRange { set { solidRange = value; } get { return solidRange; } } [SerializeField] private int solidRange;

		/// <summary>This allows you to control how the alphaTex pixels are handled.</summary>
		public PixelsType Pixels { set { pixels = value; } get { return pixels; } } [SerializeField] private PixelsType pixels;

		/// <summary>This keeps your destructible sprite active, but prevents it from taking visual damage.</summary>
		public bool Indestructible { set { indestructible = value; } get { return indestructible; } } [SerializeField] private bool indestructible;

		/// <summary>If you enable this then all destroyed or healed pixel will be sent to the <b>OnModifiedPixels</b> event, which contains a list of the <b>AlphaData</b> array indices of the pixels.</summary>
		public bool MonitorPixels { set { monitorPixels = value; } get { return monitorPixels; } } [SerializeField] private bool monitorPixels;

		/// <summary>If this destructible has been generated correctly, this will be set.</summary>
		public bool Ready { get { return ready; } } [SerializeField] protected bool ready;

		/// <summary>This stores the current visual damage state of the destructible.</summary>
		public Color32[] AlphaData { set { alphaData = value; } get { return alphaData; } } [SerializeField] protected Color32[] alphaData;

		/// <summary>This stores the current width of the visual damage data.</summary>
		public int AlphaWidth { set { alphaWidth = value; } get { return alphaWidth; } } [SerializeField] protected int alphaWidth;

		/// <summary>This stores the current height of the visual damage data.</summary>
		public int AlphaHeight { set { alphaHeight = value; } get { return alphaHeight; } } [SerializeField] protected int alphaHeight;

		/// <summary>This tells you how many pixels in the alphaData/alphaTex are solid (above 127).</summary>
		public int AlphaCount { set { alphaCount = value; } get { if (alphaCount == -1) CalculateAlphaCount(); return alphaCount; } } [SerializeField] private int alphaCount;

		/// <summary>This allows you to read the alphaCount value directly without causing it to be recalculated.</summary>
		public int AlphaCountRaw { set { alphaCount = value; } get { return alphaCount; } }

		/// <summary>This tells you the original AlphaCount value, if it was set.</summary>
		public int OriginalAlphaCount { set { originalAlphaCount = value; } get { if (originalAlphaCount == -1) CalculateAlphaCount(); return originalAlphaCount; } } [SerializeField] protected int originalAlphaCount;

		/// <summary>This allows you to read the originalAlphaCount value directly without causing it to be recalculated.</summary>
		public int OriginalAlphaCountRaw { set { originalAlphaCount = value; } get { return originalAlphaCount; } }

		/// <summary>This will return the ratio of remaining alpha (0 = no pixels remain, 1 = all pixels remain).</summary>
		public float AlphaRatio { get { return CwHelper.Divide(AlphaCount, OriginalAlphaCount); } }

		/// <summary>This tells you offset of the alpha data in local space.</summary>
		public Vector2 AlphaOffset { set { alphaOffset = value; } get { return alphaOffset; } } [SerializeField] protected Vector2 alphaOffset;

		/// <summary>This tells you scale of the alpha data in local space.</summary>
		public Vector2 AlphaScale { set { alphaScale = value; } get { return alphaScale; } } [SerializeField] protected Vector2 alphaScale;

		/// <summary>Each time you optimize/halve this sprite, this value will double.</summary>
		public float AlphaSharpness { get { return alphaSharpness; } } [SerializeField] protected float alphaSharpness;

		/// <summary>This stores the current texture of the visual destruction state.</summary>
		public Texture2D AlphaTex { get { return alphaTex; } } [System.NonSerialized] private Texture2D alphaTex;

		/// <summary>This stores the pixel region of the alphaData that hasn't been copied to the texture yet. These pixels will be copied in LateUpdate.</summary>
		[System.NonSerialized]
		public D2dRect AlphaModified;

		// Set while Split is being invoked, to prevent infinite cycles and such
		[System.NonSerialized]
		public bool IsSplitting;

		// Is OnStartSplit currently being invoked? (used to prevent collider generation issues)
		[System.NonSerialized]
		public bool IsOnStartSplit;

		[System.NonSerialized]
		private static MaterialPropertyBlock propertyBlock;

		[System.NonSerialized]
		private static bool propertyBlockSet;

		[System.NonSerialized]
		private static List<int> modifiedPixels = new List<int>();

		private static int _D2dAlpha     = Shader.PropertyToID("_D2dAlpha");
		private static int _D2dScale     = Shader.PropertyToID("_D2dScale");
		private static int _D2dOffset    = Shader.PropertyToID("_D2dOffset");
		private static int _D2dSharpness = Shader.PropertyToID("_D2dSharpness");

		/// <summary>This returns true if the healSnapshot is in a valid state for healing this destructible sprite.</summary>
		public bool CanHeal
		{
			get
			{
				if (healSnapshot != null)
				{
					var data = healSnapshot.DataRaw;

					if (data != null && data.Ready == true && data.AlphaWidth == alphaWidth && data.AlphaHeight == alphaHeight)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>This tells you the format the alphaTex should have based on your settings.</summary>
		public abstract TextureFormat FinalFormat
		{
			get;
		}

		public abstract bool InvalidMaterial
		{
			get;
		}

		public abstract Renderer CachedRenderer
		{
			get;
		}

		public abstract void ChangeMaterial();

		/// <summary>This allows you to cut and smooth the edges of your destructible. This is automatically done in many cases, but when making a new destructible it isn't, so you can control how the edges look yourself.</summary>
		[ContextMenu("Trim")]
		public void Trim()
		{
			if (ready == true)
			{
				D2dTrim.Trim(this);

				NotifyRebuilt();
			}
		}

		/// <summary>This allows you to blur the pixels in your current destruction state. This can be used for certain effects, or to smooth the edges.</summary>
		[ContextMenu("Blur")]
		public void Blur()
		{
			if (ready == true)
			{
				D2dBlur.Blur(this);

				NotifyRebuilt();
			}
		}

		/// <summary>This allows you to threshold all the pixels in your current destruction state. This will set them to full opacity if they are above half opacity, otherwise they will be set to zero opacity.</summary>
		[ContextMenu("Threshold")]
		public void Threshold()
		{
			if (ready == true)
			{
				for (var i = alphaWidth * alphaHeight - 1; i >= 0; i--)
				{
					var pixel = alphaData[i];

					pixel.a = pixel.a > 127 ? (byte)255 : (byte)0;

					alphaData[i] = pixel;
				}

				NotifyRebuilt();
			}
		}

		/// <summary>This allows you to halve the width & height of your destruction pixels.</summary>
		[ContextMenu("Halve")]
		public void Halve()
		{
			if (ready == true)
			{
				D2dHalve.Halve(ref alphaData, ref alphaWidth, ref alphaHeight, ref alphaOffset, ref alphaScale);

				alphaSharpness    *= 2;
				originalAlphaCount = CalculateAlphaCount();

				NotifyRebuilt();
			}
		}

		/// <summary>This allows you to reduce the amount of pixels used to store the destruction state of your sprite. Each time you do this you will increase performance 4x, but there will be some visual quality loss.</summary>
		[ContextMenu("Optimize")]
		public void Optimize()
		{
			if (ready == true)
			{
				D2dTrim.Trim(this);
				D2dBlur.Blur(this);
				D2dHalve.Halve(ref alphaData, ref alphaWidth, ref alphaHeight, ref alphaOffset, ref alphaScale);
				D2dTrim.Trim(this);

				alphaSharpness    *= 2;
				originalAlphaCount = CalculateAlphaCount();

				NotifyRebuilt();
			}
		}

		/// <summary>This allows you to clear all destruction data from the sprite, reverting it to a normal non-destructible sprite. NOTE: You will need to manually revert the material to completely revert the sprite state.</summary>
		[ContextMenu("Clear")]
		public void Clear()
		{
			ready              = false;
			alphaSharpness     = 0.0f;
			alphaTex           = CwHelper.Destroy(alphaTex);
			alphaData          = null;
			alphaWidth         = 0;
			alphaHeight        = 0;
			alphaCount         = 0;
			originalAlphaCount = 0;

			AlphaModified.Clear();

			NotifyRebuilt();
		}

		public void RebuildAlphaTex()
		{
			if (alphaTex != null)
			{
				var format = FinalFormat;

				if (alphaTex.format != format)
				{
					alphaTex.Reinitialize(alphaWidth, alphaHeight, format, false);
				}
			}
		}

		/// <summary>Call this if you manually modified the whole destruction state.</summary>
		public void NotifyRebuilt()
		{
			if (ready == true && alphaTex != null)
			{
				var format = FinalFormat;

				if (alphaTex.width != alphaWidth || alphaTex.height != alphaHeight || alphaTex.format != format)
				{
					alphaTex.Reinitialize(alphaWidth, alphaHeight, format, false);
				}

				alphaTex.SetPixels32(alphaData);
				alphaTex.Apply();

				AlphaModified.Clear();
			}

			if (OnRebuilt != null)
			{
				OnRebuilt();
			}
		}

		private void NotifyModified(D2dRect rect)
		{
			AlphaModified.Clear();

			if (OnModified != null)
			{
				OnModified(rect);
			}
		}

		protected int CalculateAlphaCount()
		{
			alphaCount = 0;

			if (ready == true)
			{
				var total = alphaWidth * alphaHeight;

				for (var i = 0; i < total; i++)
				{
					if (alphaData[i].a >= 128)
					{
						alphaCount++;
					}
				}
			}

			return alphaCount;
		}

		/// <summary>This matrix allows you to transform a point from local space to alpha space.
		/// NOTE: Alpha space is where 0,0 is the bottom left (first) pixel, and 1,1 is the top right (last) pixel.
		/// NOTE: Depending on your AlphaWidth and AlphaHeight, the center of the pixels will be offset half a pixel in from each corner.</summary>
		public Matrix4x4 LocalToAlphaMatrix
		{
			get
			{
				if (ready == true)
				{
					var matrix1 = Matrix4x4.identity;
					var matrix2 = Matrix4x4.identity;

					matrix1.m00 = CwHelper.Reciprocal(alphaScale.x);
					matrix1.m11 = CwHelper.Reciprocal(alphaScale.y);

					matrix2.m03 = -alphaOffset.x;
					matrix2.m13 = -alphaOffset.y;

					return matrix1 * matrix2;
				}

				return Matrix4x4.identity;
			}
		}

		/// <summary>This matrix allows you to transform a point from world space to alpha space.
		/// NOTE: Alpha space is where 0,0 is the bottom left (first) pixel, and 1,1 is the top right (last) pixel.
		/// NOTE: Depending on your AlphaWidth and AlphaHeight, the center of the pixels will be offset half a pixel in from each corner.</summary>
		public Matrix4x4 WorldToAlphaMatrix
		{
			get
			{
				if (ready == true)
				{
					return LocalToAlphaMatrix * transform.worldToLocalMatrix;
				}

				return Matrix4x4.identity;
			}
		}

		/// <summary>This matrix allows you to transform a point from alpha space to local space.
		/// NOTE: Alpha space is where 0,0 is the bottom left (first) pixel, and 1,1 is the top right (last) pixel.
		/// NOTE: Depending on your AlphaWidth and AlphaHeight, the center of the pixels will be offset half a pixel in from each corner.</summary>
		public Matrix4x4 AlphaToLocalMatrix
		{
			get
			{
				if (ready == true)
				{
					var matrix1 = Matrix4x4.identity;
					var matrix2 = Matrix4x4.identity;

					matrix1.m00 = alphaScale.x;
					matrix1.m11 = alphaScale.y;

					matrix2.m03 = alphaOffset.x;
					matrix2.m13 = alphaOffset.y;

					return matrix2 * matrix1;
				}

				return Matrix4x4.identity;
			}
		}

		/// <summary>This matrix allows you to transform a point from pixel space to local space.
		/// NOTE: Alpha space is where 0,0 is the bottom left (first) pixel, and AlphaWidth-1,AlphaHeight-1 is the top right (last) pixel.</summary>
		public Matrix4x4 PixelToLocalMatrix
		{
			get
			{
				if (ready == true)
				{
					var matrix1 = Matrix4x4.identity;
					var matrix2 = Matrix4x4.identity;

					matrix1.m00 = CwHelper.Reciprocal(alphaWidth );
					matrix1.m11 = CwHelper.Reciprocal(alphaHeight);

					matrix2.m03 = 0.5f;
					matrix2.m13 = 0.5f;

					return AlphaToLocalMatrix * matrix1 * matrix2;
				}

				return Matrix4x4.identity;
			}
		}

		/// <summary>This matrix allows you to transform a point from pixel space to world space.
		/// NOTE: Alpha space is where 0,0 is the bottom left (first) pixel, and AlphaWidth-1,AlphaHeight-1 is the top right (last) pixel.</summary>
		public Matrix4x4 PixelToWorldMatrix
		{
			get
			{
				return transform.localToWorldMatrix * PixelToLocalMatrix;
			}
		}

		public void SubsetAlphaWith(Color32[] subData, D2dRect subRect, int newAlphaCount = -1)
		{
			var stepX = CwHelper.Divide(alphaScale.x, alphaWidth );
			var stepY = CwHelper.Divide(alphaScale.y, alphaHeight);

			alphaOffset.x += stepX * subRect.MinX;
			alphaOffset.y += stepY * subRect.MinY;
			alphaScale.x  += stepX * (subRect.SizeX - alphaWidth );
			alphaScale.y  += stepY * (subRect.SizeY - alphaHeight);

			FastCopyAlphaData(subData, subRect.SizeX, subRect.SizeY, newAlphaCount);

			NotifyRebuilt();
		}

		private void FastCopyAlphaData(Color32[] newAlphaData, int newAlphaWidth, int newAlphaHeight, int newAlphaCount = -1)
		{
			var newAlphaTotal = newAlphaWidth * newAlphaHeight;

			if (alphaData == null || alphaData.Length != newAlphaTotal)
			{
				alphaData = new Color32[newAlphaTotal];
			}

			for (var i = newAlphaTotal - 1; i >= 0; i--)
			{
				alphaData[i] = newAlphaData[i];
			}

			alphaWidth  = newAlphaWidth;
			alphaHeight = newAlphaHeight;
			alphaCount  = newAlphaCount;
		}

		private static List<D2dDestructible> splitDestructibles = new List<D2dDestructible>();

		public void SplitBegin()
		{
			splitDestructibles.Clear();

			if (OnSplitStart != null)
			{
				OnSplitStart();
			}

			alphaData = null;
		}

		public D2dDestructible SplitNext(bool isLast)
		{
			var splitDestructible = default(D2dDestructible);

			if (isLast == true)
			{
				splitDestructible = this;
			}
			else
			{
				splitDestructible = Instantiate(this, transform.localPosition, transform.localRotation);

				splitDestructible.transform.SetParent(transform.parent, false);
			}

			splitDestructibles.Add(splitDestructible);

			return splitDestructible;
		}

		public void SplitEnd(SplitMode split)
		{
			if (OnSplitEnd != null)
			{
				OnSplitEnd(splitDestructibles, split);
			}
		}

		/// <summary>This method allows you to sample the <b>AlphaData</b> at the specified local space position.</summary>
		public Color32 SampleAlphaLocal(Vector3 localPosition)
		{
			return SampleAlpha(LocalToAlphaMatrix.MultiplyPoint(localPosition));
		}

		/// <summary>This method allows you to sample the <b>AlphaData</b> at the specified world space position.</summary>
		public Color32 SampleAlphaWorld(Vector3 worldPosition)
		{
			return SampleAlpha(WorldToAlphaMatrix.MultiplyPoint(worldPosition));
		}

		/// <summary>This method allows you to sample the <b>AlphaData</b> at the specified UV.</summary>
		public Color32 SampleAlpha(Vector2 uv)
		{
			if (uv.x >= 0.0f && uv.y >= 0.0f && uv.x < 1.0f && uv.y < 1.0f)
			{
				var x = Mathf.FloorToInt(uv.x * alphaWidth );
				var y = Mathf.FloorToInt(uv.y * alphaHeight);

				return alphaData[x + y * alphaWidth];
			}

			return default(Color32);
		}

		/// <summary>This method allows you to find the destructible object below the specified world space position as long as the sampled alpha value is above the threshold value.
		/// This allows you to go through holes in objects that are on top.</summary>
		public static bool TrySampleThrough(Vector3 worldPosition, ref D2dDestructible hitDestructible, byte threshold = 127)
		{
			var hit          = false;
			var destructible = FirstInstance;

			for (var i = 0; i < InstanceCount; i++)
			{
				var alpha = destructible.SampleAlphaWorld(worldPosition);

				if (alpha.a > threshold)
				{
					if (hit == false || destructible.IsAbove(hitDestructible) == true)
					{
						hit             = true;
						hitDestructible = destructible;
					}
				}

				destructible = destructible.NextInstance;
			}

			return hit;
		}

		/// <summary>This method allows you to see if the current destructible object is being drawn above the specified object.</summary>
		public bool IsAbove(D2dDestructible other)
		{
			if (other != null)
			{
				var cur = CachedRenderer;
				var oth = other.CachedRenderer;

				return cur.sortingLayerID >= oth.sortingLayerID && cur.sortingOrder >= oth.sortingOrder;
			}

			return false;
		}

		/// <summary>This method allows you to sample the <b>AlphaData</b> of the top most object at the specified world space position.</summary>
		public static Color32 TrySampleAlphaAll(Vector3 worldPosition)
		{
			var destructible = default(D2dDestructible);
			var alpha        = default(Color32);

			TrySampleAlphaAll(worldPosition, ref destructible, ref alpha);

			return alpha;
		}

		/// <summary>This method allows you to sample the <b>AlphaData</b> of the top most object at the specified world space position and also find out which destructible object it came from.</summary>
		public static bool TrySampleAlphaAll(Vector3 worldPosition, ref D2dDestructible hitDestructible, ref Color32 hitAlpha)
		{
			var hit          = false;
			var destructible = FirstInstance;

			for (var i = 0; i < InstanceCount; i++)
			{
				var alpha = destructible.SampleAlphaWorld(worldPosition);

				if (alpha.a > 0.0f)
				{
					if (hit == false || destructible.IsAbove(hitDestructible) == true)
					{
						hit             = true;
						hitDestructible = destructible;
						hitAlpha        = alpha;
					}
				}

				destructible = destructible.NextInstance;
			}

			return hit;
		}

		protected virtual void LateUpdate()
		{
			if (ready == true && AlphaModified.IsSet == true && alphaTex != null)
			{
				var w = AlphaModified.SizeX;
				var h = AlphaModified.SizeY;

				// Replace all pixels?
				if (w * h > 1000)
				{
					alphaTex.SetPixels32(alphaData);
					alphaTex.Apply();
				}
				// Replace updated pixels?
				else
				{
					var i = 0;

					D2dCommon.ReserveTempAlphaData(w, h);

					for (var y = AlphaModified.MinY; y < AlphaModified.MaxY; y++)
					{
						var o = y * alphaWidth;

						for (var x = AlphaModified.MinX; x < AlphaModified.MaxX; x++)
						{
							D2dCommon.tempAlphaData[i++] = alphaData[o + x];
						}
					}

					alphaTex.SetPixels32(AlphaModified.MinX, AlphaModified.MinY, w, h, D2dCommon.tempAlphaData);
					alphaTex.Apply();
				}

				NotifyModified(AlphaModified);
			}
		}

		protected virtual void Destroy()
		{
			CwHelper.Destroy(alphaTex);
		}

		private static List<Material> tempMaterials = new List<Material>();

		protected virtual void OnWillRenderObject(Renderer renderer)
		{
			if (ready == true)
			{
				if (alphaTex == null)
				{
					alphaTex = new Texture2D(alphaWidth, alphaHeight, FinalFormat, false);

					alphaTex.wrapMode = TextureWrapMode.Clamp;

					alphaTex.SetPixels32(alphaData);
					alphaTex.Apply();

					AlphaModified.Clear();
				}

				alphaTex.filterMode = pixels == PixelsType.Smooth ? FilterMode.Bilinear : FilterMode.Point;

				if (propertyBlockSet == false)
				{
					propertyBlock    = new MaterialPropertyBlock();
					propertyBlockSet = true;
				}

				renderer.GetSharedMaterials(tempMaterials);

				if (tempMaterials.Count > 1)
				{
					for (var i = 0; i < tempMaterials.Count; i++)
					{
						renderer.GetPropertyBlock(propertyBlock, i);

						ApplyProperties();

						renderer.SetPropertyBlock(propertyBlock, i);
					}
				}
				else
				{
					renderer.GetPropertyBlock(propertyBlock);

					ApplyProperties();

					renderer.SetPropertyBlock(propertyBlock);
				}
			}
		}

		private void ApplyProperties()
		{
			propertyBlock.SetTexture(_D2dAlpha, alphaTex);
			propertyBlock.SetVector(_D2dScale, alphaScale);
			propertyBlock.SetVector(_D2dOffset, alphaOffset);

			if (overrideSharpness == 0.0f)
			{
				propertyBlock.SetFloat(_D2dSharpness, alphaSharpness);
			}
			else if (overrideSharpness > 0.0f)
			{
				propertyBlock.SetFloat(_D2dSharpness, overrideSharpness);
			}
			else
			{
				propertyBlock.SetFloat(_D2dSharpness, alphaSharpness * -overrideSharpness);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dDestructible;

	[CanEditMultipleObjects]
	public class D2dDestructible_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var rebuild = false;

			if (Any(tgts, t => t.GetComponent<PolygonCollider2D>() != null))
			{
				Warning("D2dDestructible isn't compatible with PolygonCollider2D, use D2dPolygonCollider instead.");
			}

			if (Any(tgts, t => t.GetComponent<EdgeCollider2D>() != null))
			{
				Warning("D2dDestructible isn't compatible with EdgeCollider2D, use D2dEdgeCollider instead.");
			}

			Draw("healSnapshot", "If you want to be able to heal this destructible sprite, then set a snapshot of the healed state here.");

			if (Any(tgts, t => t.HealSnapshot != null && t.CanHeal == false))
			{
				Warning("This healSnapshot is incompatible with this destructible sprite state.");
			}

			Draw("overrideSharpness", "This allows you to manually control the sharpness of the alpha gradient:\nZero = AlphaSharpness\nPositive = OverrideSharpness\nNegative = AlphaSharpness * -OverrideSharpness");
			Draw("paintMultiplier", "This allows you to control how easily this object can be painted.\n\n1 = Default.\n2 = Twice as much damage.\n0.5 = Half as much damage.");
			Draw("pixels", "This allows you to control how the alphaTex pixels are handled.");
			Draw("indestructible", "This keeps your destructible sprite active, but prevents it from taking visual damage.");
			Draw("monitorPixels", "If you enable this then all destroyed or healed pixel will be sent to the OnModifiedPixels event, which contains a list of the AlphaData array indices of the pixels.");

			Separator();

			EditorGUILayout.BeginHorizontal();
				if (Any(tgts, t => t.InvalidMaterial))
				{
					if (GUILayout.Button("Change Material") == true)
					{
						Each(tgts, t => { if (t.InvalidMaterial == true) t.ChangeMaterial(); });
					}
				}

				if (GUILayout.Button("Optimize") == true)
				{
					Each(tgts, t => t.Optimize(), true);
				}

				if (GUILayout.Button("Trim") == true)
				{
					Each(tgts, t => t.Trim(), true);
				}

				if (GUILayout.Button("Clear") == true)
				{
					Each(tgts, t => t.Clear(), true);
				}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
				if (Any(tgts, t => t.GetComponent<D2dCollider>() == null))
				{
					if (GUILayout.Button("+ Polygon Collider") == true)
					{
						Each(tgts, t => t.gameObject.AddComponent<D2dPolygonCollider>());
					}

					if (GUILayout.Button("+ Edge Collider") == true)
					{
						Each(tgts, t => t.gameObject.AddComponent<D2dEdgeCollider>());
					}
				}

				if (Any(tgts, t => t.GetComponent<D2dSplitter>() == null) && GUILayout.Button("+ Splitter") == true)
				{
					Each(tgts, t => t.gameObject.AddComponent<D2dSplitter>());
				}
			EditorGUILayout.EndHorizontal();

			Separator();

			if (tgts.Length == 1)
			{
				BeginDisabled();
					EditorGUI.ObjectField(Reserve(), "Alpha Tex", tgt.AlphaTex, typeof(Texture2D), true);
					EditorGUILayout.IntField("Alpha Width", tgt.AlphaWidth);
					EditorGUILayout.IntField("Alpha Height", tgt.AlphaHeight);
					EditorGUILayout.FloatField("Alpha Sharpness", tgt.AlphaSharpness);
					EditorGUILayout.IntField("Alpha Count", tgt.AlphaCount);
					EditorGUILayout.IntField("Original Alpha Count", tgt.OriginalAlphaCount);
					EditorGUI.ProgressBar(Reserve(), tgt.AlphaRatio, "Alpha Ratio");
				EndDisabled();
			}

			if (rebuild == true)
			{
				Each(tgts, t => t.RebuildAlphaTex(), true);
			}
		}
	}
}
#endif