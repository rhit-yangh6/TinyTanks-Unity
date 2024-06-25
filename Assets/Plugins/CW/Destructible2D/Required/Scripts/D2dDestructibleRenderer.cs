using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to turn any Renderer into a destructible one. The destruction is stored using a copy of the alpha/opacity of the original renderer, and you have many options to reduce/optimize the amount of destruction pixels used, as well as cut holes in the data.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dDestructibleRenderer")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Destructible Renderer")]
	public class D2dDestructibleRenderer : D2dDestructible
	{
		public enum ChannelType
		{
			AlphaOnly,
			AlphaWithWhiteRGB,
			FullRGBA
		}

		/// <summary>To rebuild this destructible object its original (non-destructible) material is used.</summary>
		public List<Material> OriginalMaterials { get { if (originalMaterials == null) originalMaterials = new List<Material>(); return originalMaterials; } } [SerializeField] private List<Material> originalMaterials;

		/// <summary>This allows you to set which color channels you want the destructible texture to use.</summary>
		public ChannelType Channels { set { if (value != channels) { channels = value; RebuildAlphaTex(); } } get { return channels; } } [SerializeField] private ChannelType channels = ChannelType.AlphaWithWhiteRGB;

		/// <summary>This allows you to set channels without triggering a rebuild.</summary>
		public ChannelType ChannelsRaw { set { channels = value; } get { return channels; } }

		/// <summary>When this destructible object is rebuilt, it will be rendered using a temporary camera with everything set to this layer.
		/// NOTE: There must be no visible objects on this layer, otherwise this component will not function properly.</summary>
		public int RebuildLayer { set { rebuildLayer = value; } get { return rebuildLayer; } } [SerializeField] [Range(0, 31)] private int rebuildLayer = 31;

		/// <summary>To save scene file size you can Clear your destructible, and allow it to Rebuilt on Start.</summary>
		public bool RebuildInGame { set { rebuildInGame = value; } get { return rebuildInGame; } } [SerializeField] private bool rebuildInGame;

		/// <summary>This allows you to set how many times the rebuilt alpha data will be optimized when rebuilt on Start.</summary>
		public int RebuildOptimizeCount { set { rebuildOptimizeCount = value; } get { return rebuildOptimizeCount; } } [SerializeField] private int rebuildOptimizeCount;

		/// <summary>This allows you to set how many destructible pixels are generated per unit in local space. For example, if your object is 10 units in width + height, and this value is set to 3, then this destructible object will have a resolution of 30x30 pixels.</summary>
		public int PixelsPerUnit { set { pixelsPerUnit = value; } get { return pixelsPerUnit; } } [SerializeField] private int pixelsPerUnit = 10;

		[System.NonSerialized]
		protected Renderer cachedRenderer;

		[System.NonSerialized]
		private bool cachedRendererSet;

		/// <summary>This gives you the attached Renderer.</summary>
		public override Renderer CachedRenderer
		{
			get
			{
				if (cachedRendererSet == false)
				{
					cachedRenderer    = GetComponent<Renderer>();
					cachedRendererSet = true;
				}

				return cachedRenderer;
			}
		}

		/// <summary>This tells you if the attached Renderer's sharedMaterial uses the default Unity sprite material, which isn't compatible with destructible objects.</summary>
		public override bool InvalidMaterial
		{
			get
			{
				var material = CachedRenderer.sharedMaterial;

				return material == null || material.shader == null || material.shader.name == "Sprites/Default";
			}
		}

		public override TextureFormat FinalFormat
		{
			get
			{
				return channels == ChannelType.AlphaOnly ? TextureFormat.Alpha8 : TextureFormat.ARGB32;
			}
		}

		private static List<Material> tempMaterials = new List<Material>();

		/// <summary>If you're using the normal Unity sprite material, then this swaps it to the Destructible 2D supported equivalent.</summary>
		public override void ChangeMaterial()
		{
			CachedRenderer.GetSharedMaterials(tempMaterials);

			if (OriginalMaterials.Count == 0)
			{
				cachedRenderer.GetSharedMaterials(originalMaterials);
			}

			var defaultMaterial = Resources.Load<Material>("Destructible 2D/D2D Default");

			if (tempMaterials.Count == 0)
			{
				tempMaterials.Add(defaultMaterial);
			}
			else
			{
				for (var i = 0; i < tempMaterials.Count; i++)
				{
					tempMaterials[i] = defaultMaterial;
				}
			}

			cachedRenderer.sharedMaterials = D2dCache.CachedList<Material>.ToArray(tempMaterials);
		}

		/// <summary>This allows you to rebuild the destruction state using the current sprite settings.</summary>
		[ContextMenu("Rebuild")]
		public void Rebuild()
		{
			Rebuild(0);
		}

		/// <summary>This method allows you to revert the associated renderer's materials to the original values before you made it destructible.</summary>
		[ContextMenu("Revert Materials")]
		public void RevertMaterials()
		{
			if (originalMaterials != null)
			{
				cachedRenderer.GetSharedMaterials(tempMaterials);

				cachedRenderer.sharedMaterials = D2dCache.CachedList<Material>.ToArray(originalMaterials);

				originalMaterials.Clear();
				originalMaterials.AddRange(tempMaterials);
			}
		}

		private static Material cachedTexture;

		private static bool cachedTextureSet;

		/// <summary>This allows you to rebuild the destruction state using the specified sprites.</summary>
		public void Rebuild(int optimizeCount)
		{
			var layer = 31;

			var renderer    = CachedRenderer;
			var bounds      = renderer.bounds;
			var offset      = bounds.min - transform.position;
			var camera      = new GameObject("Camera").AddComponent<Camera>();
			var width       = Mathf.CeilToInt(bounds.extents.x * pixelsPerUnit);
			var height      = Mathf.CeilToInt(bounds.extents.y * pixelsPerUnit);
			var desc        = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
			var target      = CwRenderTextureManager.GetTemporary(desc, "D2dDestructibleRenderer Rebuild");
			var oldActive   = RenderTexture.active;
			var oldLayer    = gameObject.layer;
			var buffer      = new Texture2D(width, height, TextureFormat.ARGB32, false);

			target.DiscardContents();

			camera.orthographic       = true;
			camera.orthographicSize   = bounds.extents.y;
			camera.targetTexture      = target;
			camera.nearClipPlane      = -1.0f;
			camera.farClipPlane       = 1.0f;
			camera.transform.position = bounds.center;
			camera.cullingMask        = 1 << layer;
			camera.clearFlags         = CameraClearFlags.Nothing;

			gameObject.layer = layer;
				renderer.GetSharedMaterials(tempMaterials);
				renderer.sharedMaterials = D2dCache.CachedList<Material>.ToArray(originalMaterials);
					camera.Render();
				renderer.sharedMaterials = D2dCache.CachedList<Material>.ToArray(tempMaterials);
			gameObject.layer = oldLayer;

			RenderTexture.active = target;

			buffer.ReadPixels(new Rect(0, 0, width, height), 0, 0);

			RenderTexture.active = oldActive;

			ready          = true;
			alphaData      = buffer.GetPixels32();
			alphaWidth     = width;
			alphaHeight    = height;
			alphaOffset.x  = offset.x;
			alphaOffset.y  = offset.y;
			alphaScale.x   = bounds.size.x;
			alphaScale.y   = bounds.size.y;
			alphaSharpness = 1.0f;

			DestroyImmediate(buffer);
			CwRenderTextureManager.ReleaseTemporary(target);
			DestroyImmediate(camera.gameObject);

			originalAlphaCount = CalculateAlphaCount();

			if (channels == ChannelType.AlphaWithWhiteRGB)
			{
				var total = width * height;

				for (var i = 0; i < total; i++)
				{
					var alphaPixel = alphaData[i]; alphaData[i] = new Color32(255, 255, 255, alphaPixel.a);
				}
			}

			for (var i = 0; i < optimizeCount; i++)
			{
				Optimize();
			}

			NotifyRebuilt();
		}

		protected virtual void Start()
		{
			// Auto upgrade data?
#if UNITY_EDITOR
			if (ready == true && alphaScale.x == 0.0f && alphaScale.y == 0.0f && Application.isPlaying == false)
			{
				Rebuild(Mathf.RoundToInt(Mathf.Log(alphaSharpness, 2.0f)));

				UnityEditor.EditorUtility.SetDirty(this);
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
			}

			if (Application.isPlaying == false)
			{
				return;
			}
#endif
			if (rebuildInGame == true)
			{
				Rebuild(rebuildOptimizeCount);
			}
		}

		[System.NonSerialized]
		private static MaterialPropertyBlock propertyBlock;

		[System.NonSerialized]
		private static bool propertyBlockSet;

		protected virtual void OnWillRenderObject()
		{
			var renderer = CachedRenderer;

			OnWillRenderObject(renderer);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dDestructibleRenderer;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDestructibleRenderer_Editor : D2dDestructible_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var rebuild = false;

			BeginError(Any(tgts, t => t.OriginalMaterials.Count == 0));
				Draw("originalMaterials", "To rebuild this destructible object its original (non-destructible) material is used.");
			EndError();
			Draw("channels", ref rebuild, "This allows you to set which color channels you want the destructible texture to use.");
			Draw("solidRange", "This allows you to make it so some pixels are harder to destroy than others, based on their alpha value (0 .. 255).\n\n0 = Every pixel is equally easy to destroy.\n\n1 = Values between 0 and 254 receive normal damage, but 255 receives no damage.\n\n10 = Values between 0 and 245 receive normal damage, but 246 receives 90% damage, 247 receives 80%, etc.");
			if (Any(tgts, t => t.SolidRange > 0))
			{
				BeginIndent();
					Draw("solidRangeMask", "This mask can be used to more easily control which pixel alpha values fall into the SolidRange.\n\n0 Alpha = This pixel will receive normal damage.\n\n255 Alpha = This pixel will receive no damage.", "Mask");
				EndIndent();
			}
			Draw("rebuildLayer", "When this destructible object is rebuilt, it will be rendered using a temporary camera with everything set to this layer.\n\nNOTE: There must be no visible objects on this layer, otherwise this component will not function properly.");
			Draw("rebuildInGame", "To save scene file size you can Clear your destructible, and allow it to Rebuilt on Start.");
			Draw("pixelsPerUnit", "This allows you to set how many destructible pixels are generated per unit in local space. For example, if your object is 10 units in width + height, and this value is set to 3, then this destructible object will have a resolution of 30x30 pixels.");

			if (Any(tgts, t => t.RebuildInGame == true))
			{
				if (Application.isPlaying == false && Any(tgts, t => t.Ready == true))
				{
					Warning("If you want your destructible sprite to be rebuilt in game, then you should click the Clear button in edit mode.");
				}
				BeginIndent();
					Draw("rebuildOptimizeCount", "This allows you to set how many times the rebuilt alpha data will be optimized when rebuilt on Start.");
				EndIndent();
			}

			if (Any(tgts, t => t.Ready == false && t.RebuildInGame == false))
			{
				Warning("If you want your destructible sprite to be rebuilt on Play, then you should Clear it in edit mode.");
			}

			if (Button("Rebuild") == true)
			{
				Each(tgts, t => t.Rebuild());
			}

			if (Button("Revert Materials") == true)
			{
				Each(tgts, t => t.RevertMaterials());
			}

			Separator();

			base.OnInspector();

			if (rebuild == true)
			{
				Each(tgts, t => t.Rebuild());
			}
		}
	}
}
#endif