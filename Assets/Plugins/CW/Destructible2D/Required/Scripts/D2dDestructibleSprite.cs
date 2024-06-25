using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to turn a normal SpriteRenderer into a destructible one. The destruction is stored using a copy of the alpha/opacity of the original sprite, and you have many options to reduce/optimize the amount of destruction pixels used, as well as cut holes in the data.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dDestructibleSprite")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Destructible Sprite")]
	public class D2dDestructibleSprite : D2dDestructible
	{
		public enum ChannelType
		{
			AlphaOnly,
			AlphaWithWhiteRGB,
			FullRGBA
		}

		/// <summary>This allows you to set the shape of the destructible sprite.\nNOTE: This should match the settings of your visual sprite.</summary>
		public Sprite Shape { set { shape = value; } get { return shape; } } [SerializeField] private Sprite shape;

		/// <summary>This allows you to override the sprite texture with any Texture.</summary>
		public Texture OverrideTexture { set { overrideTexture = value; } get { return overrideTexture; } } [SerializeField] private Texture overrideTexture;

		/// <summary>This mask can be used to more easily control which pixel alpha values fall into the <b>SolidRange</b>.
		/// 0 Alpha = This pixel will receive normal damage.
		/// 255 Alpha = This pixel will receive no damage.</summary>
		public Sprite SolidRangeMask { set { solidRangeMask = value; } get { return solidRangeMask; } } [SerializeField] private Sprite solidRangeMask;

		/// <summary>This allows you to set which color channels you want the destructible texture to use.</summary>
		public ChannelType Channels { set { if (value != channels) { channels = value; RebuildAlphaTex(); } } get { return channels; } } [SerializeField] private ChannelType channels;

		/// <summary>This allows you to set channels without triggering a rebuild.</summary>
		public ChannelType ChannelsRaw { set { channels = value; } get { return channels; } }

		/// <summary>Enable this if you want the attached SpriteRenderer.sprite to automatically crop to the AlphaTex boundary, reducing the fill rate requirements for large splitting sprites.</summary>
		public bool CropSprite { set { if (value != cropSprite) { cropSprite = value; UpdateSprite(); } } get { return cropSprite; } } [SerializeField] private bool cropSprite = true;

		/// <summary>To save scene file size you can Clear your destructible, and allow it to Rebuilt on Start.</summary>
		public bool RebuildInGame { set { rebuildInGame = value; } get { return rebuildInGame; } } [SerializeField] private bool rebuildInGame;

		/// <summary>This allows you to set how many times the rebuilt alpha data will be optimized when rebuilt on Start.</summary>
		public int RebuildOptimizeCount { set { rebuildOptimizeCount = value; } get { return rebuildOptimizeCount; } } [SerializeField] private int rebuildOptimizeCount;

		[SerializeField]
		private Sprite originalSprite;

		[SerializeField]
		private Sprite clonedSprite;

		[System.NonSerialized]
		protected SpriteRenderer cachedSpriteRenderer;

		[System.NonSerialized]
		private bool cachedSpriteRendererSet;

		[System.NonSerialized]
		private static Vector2[] vertices = new Vector2[4];

		[System.NonSerialized]
		private static readonly ushort[] triangles = { 0, 1, 2, 3, 2, 1 };

		private static int _MainTex = Shader.PropertyToID("_MainTex");

		/// <summary>This gives you the attached SpriteRenderer.</summary>
		public SpriteRenderer CachedSpriteRenderer
		{
			get
			{
				if (cachedSpriteRendererSet == false)
				{
					cachedSpriteRenderer    = GetComponent<SpriteRenderer>();
					cachedSpriteRendererSet = true;
				}

				return cachedSpriteRenderer;
			}
		}

		public override Renderer CachedRenderer
		{
			get
			{
				return CachedSpriteRenderer;
			}
		}

		/// <summary>This tells you if the attached SpriteRenderer's sharedMaterial uses the default Unity sprite material, which isn't compatible with destructible objects.</summary>
		public override bool InvalidMaterial
		{
			get
			{
				var material = CachedSpriteRenderer.sharedMaterial;

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

		/// <summary>If you're using the normal Unity sprite material, then this swaps it to the Destructible 2D supported equivalent.</summary>
		public override void ChangeMaterial()
		{
			CachedSpriteRenderer.sharedMaterial = Resources.Load<Material>("Destructible 2D/D2D Default");
		}

		public void UpdateSprite()
		{
			if (cropSprite == true)
			{
				if (CachedSpriteRenderer.sprite == null)
				{
					cachedSpriteRenderer.sprite = originalSprite;
				}

				if (cachedSpriteRenderer.sprite != clonedSprite)
				{
					originalSprite = cachedSpriteRenderer.sprite;
					clonedSprite   = CwHelper.Destroy(clonedSprite);
					clonedSprite   = cachedSpriteRenderer.sprite = Instantiate(originalSprite);
				}
			}
			else
			{
				if (CachedSpriteRenderer.sprite == clonedSprite)
				{
					CachedSpriteRenderer.sprite = originalSprite;

					clonedSprite = CwHelper.Destroy(clonedSprite);
				}
			}
		}

		/// <summary>This allows you to rebuild the destruction state using the current sprite settings.</summary>
		[ContextMenu("Rebuild")]
		public void Rebuild()
		{
			Rebuild(0);
		}

		/// <summary>This allows you to rebuild the destruction state using the specified sprites.</summary>
		public void Rebuild(int optimizeCount)
		{
			if (shape == null)
			{
				shape = CachedSpriteRenderer.sprite;
			}

			if (shape != null)
			{
				var shapeRect = D2dCommon.SpritePixelRect(shape);
				var ppu       = shape.pixelsPerUnit;

				ready          = true;
				alphaData      = D2dCommon.ReadPixels(shape.texture, shapeRect.x, shapeRect.y, shapeRect.width, shapeRect.height);
				alphaWidth     = shapeRect.width;
				alphaHeight    = shapeRect.height;
				alphaOffset.x  = (shape.textureRectOffset.x - shape.pivot.x) / ppu;
				alphaOffset.y  = (shape.textureRectOffset.y - shape.pivot.y) / ppu;
				alphaScale.x   = shape.textureRect.width  / ppu;
				alphaScale.y   = shape.textureRect.height / ppu;
				alphaSharpness = 1.0f;

				if (solidRangeMask != null && SolidRange > 0)
				{
					var maskRect = D2dCommon.SpritePixelRect(solidRangeMask);
					var maskSize = maskRect.size;
					var maskData = D2dCommon.ReadPixels(solidRangeMask.texture, maskRect.x, maskRect.y, maskRect.width, maskRect.height);
					var maskMin  = System.Math.Max(0, 255 - SolidRange);
					var stepH    = 1.0f / alphaWidth;
					var stepV    = 1.0f / alphaHeight;

					for (var y = 0; y < alphaHeight; y++)
					{
						for (var x = 0; x < alphaWidth; x++)
						{
							var index      = x + y * alphaWidth;
							var alphaPixel = alphaData[index];

							if (alphaPixel.a > maskMin)
							{
								var maskCoord = new Vector2(x * stepH, y * stepV);
								var maskPixel = D2dCommon.Sample(maskData, maskSize, maskCoord);

								alphaPixel.a = (byte)Mathf.Lerp(maskMin, 255, maskPixel.a);

								alphaData[index] = alphaPixel;
							}
						}
					}
				}

				originalAlphaCount = CalculateAlphaCount();

				if (channels == ChannelType.AlphaWithWhiteRGB)
				{
					var total = shapeRect.width * shapeRect.height;

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
			else
			{
				Clear();
			}
		}

		protected virtual void Start()
		{
			// Auto upgrade data?
#if UNITY_EDITOR
			if (ready == true && alphaScale.x == 0.0f && alphaScale.y == 0.0f && Application.isPlaying == false)
			{
				Rebuild(Mathf.RoundToInt(Mathf.Log(alphaSharpness, 2.0f)));

				UnityEditor.EditorUtility.SetDirty(this);
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
			}
#endif
#if UNITY_EDITOR
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

		protected override void OnEnable()
		{
			base.OnEnable();

			if (cropSprite == true)
			{
				if (shape == null)
				{
					shape = CachedSpriteRenderer.sprite;
				}

				if (originalSprite != null)
				{
					clonedSprite = CachedSpriteRenderer.sprite = Instantiate(originalSprite);
				}
				else
				{
					originalSprite = CachedSpriteRenderer.sprite;

					if (originalSprite != null)
					{
						clonedSprite = cachedSpriteRenderer.sprite = Instantiate(originalSprite);
					}
					else
					{
						clonedSprite = null;
					}
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (CachedSpriteRenderer.sprite == clonedSprite)
			{
				CachedSpriteRenderer.sprite = originalSprite;
			}

			clonedSprite   = CwHelper.Destroy(clonedSprite);
			originalSprite = null;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			UpdateSprite();

			if (cropSprite == true && clonedSprite != null)
			{
				var ppu          = clonedSprite.pixelsPerUnit;
				var siz          = clonedSprite.rect.size;
				var alphaOffsetX = (clonedSprite.textureRectOffset.x - clonedSprite.pivot.x) / ppu;
				var alphaOffsetY = (clonedSprite.textureRectOffset.y - clonedSprite.pivot.y) / ppu;
				var alphaScaleX  = clonedSprite.textureRect.width  / ppu;
				var alphaScaleY  = clonedSprite.textureRect.height / ppu;

				var l = siz.x * Mathf.InverseLerp(alphaOffsetX, alphaOffsetX + alphaScaleX, alphaOffset.x);
				var b = siz.y * Mathf.InverseLerp(alphaOffsetY, alphaOffsetY + alphaScaleY, alphaOffset.y);
				var r = siz.x * Mathf.InverseLerp(alphaOffsetX, alphaOffsetX + alphaScaleX, alphaOffset.x + alphaScale.x);
				var t = siz.y * Mathf.InverseLerp(alphaOffsetY, alphaOffsetY + alphaScaleY, alphaOffset.y + alphaScale.y);

				vertices[0] = new Vector2(l, b);
				vertices[1] = new Vector2(r, b);
				vertices[2] = new Vector2(l, t);
				vertices[3] = new Vector2(r, t);

				clonedSprite.OverrideGeometry(vertices, triangles);
			}
		}

		protected virtual void OnDestroy()
		{
			CwHelper.Destroy(clonedSprite);
		}

		[System.NonSerialized]
		private static MaterialPropertyBlock propertyBlock;

		[System.NonSerialized]
		private static bool propertyBlockSet;

		protected virtual void OnWillRenderObject()
		{
			var renderer = CachedSpriteRenderer;

			OnWillRenderObject(renderer);

			if (overrideTexture != null)
			{
				if (ready == true)
				{
					if (propertyBlockSet == false)
					{
						propertyBlock    = new MaterialPropertyBlock();
						propertyBlockSet = true;
					}

					renderer.GetPropertyBlock(propertyBlock);

					propertyBlock.SetTexture(_MainTex, overrideTexture);

					renderer.SetPropertyBlock(propertyBlock);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dDestructibleSprite;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDestructibleSprite_Editor : D2dDestructible_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var rebuild      = false;
			var updateSprite = false;

			if (Any(tgts, t => t.CachedSpriteRenderer.flipX == true || t.CachedSpriteRenderer.flipY == true))
			{
				Error("D2dDestructible isn't compatible with flipped sprites, use Transform.localRotation instead.");
			}

			if (Any(tgts, t => t.CachedSpriteRenderer.drawMode != SpriteDrawMode.Simple))
			{
				Error("D2dDestructible is only compatible with Simple sprites.");
			}

			if (Any(tgts, t => t.Shape != null && t.Shape.packed == true && t.Shape.packingMode != SpritePackingMode.Tight))
			{
				Error("D2dDestructible is only compatible with sprites packed with the import setting: Mesh Type = Tight");
			}

			BeginError(Any(tgts, t => t.Shape == null));
				Draw("shape", ref rebuild, "This allows you to set the shape of the destructible sprite.\n\nNOTE: This should match the settings of your visual sprite.");
			EndError();
			Draw("overrideTexture", "This allows you to override the sprite texture with any Texture.");
			Draw("channels", ref rebuild, "This allows you to set which color channels you want the destructible texture to use.");
			Draw("solidRange", "This allows you to make it so some pixels are harder to destroy than others, based on their alpha value (0 .. 255).\n\n0 = Every pixel is equally easy to destroy.\n\n1 = Values between 0 and 254 receive normal damage, but 255 receives no damage.\n\n10 = Values between 0 and 245 receive normal damage, but 246 receives 90% damage, 247 receives 80%, etc.");
			if (Any(tgts, t => t.SolidRange > 0))
			{
				BeginIndent();
					Draw("solidRangeMask", "This mask can be used to more easily control which pixel alpha values fall into the SolidRange.\n\n0 Alpha = This pixel will receive normal damage.\n\n255 Alpha = This pixel will receive no damage.", "Mask");
				EndIndent();
			}
			Draw("cropSprite", ref updateSprite, "Enable this if you want the attached SpriteRenderer.sprite to automatically crop to the AlphaTex boundary, reducing the fill rate requirements for large splitting sprites.");
			Draw("rebuildInGame", "To save scene file size you can Clear your destructible, and allow it to Rebuilt on Start.");

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

			if (GUILayout.Button("Rebuild") == true)
			{
				Each(tgts, t => t.Rebuild(), true);
			}

			Separator();

			base.OnInspector();

			if (rebuild == true)
			{
				Each(tgts, t => t.Rebuild(), true);
			}

			if (updateSprite == true)
			{
				Each(tgts, t => t.UpdateSprite(), true);
			}
		}
	}
}
#endif