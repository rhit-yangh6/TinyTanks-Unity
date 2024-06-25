using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component shows you how to create a destructible sprite entirely from code.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dProceduralSetup")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Procedural Setup")]
	public class D2dProceduralSetup : MonoBehaviour
	{
		/// <summary>The size of the generated image.</summary>
		public int Size { set { size = value; } get { return size; } } [SerializeField] private int size = 128;

		protected virtual void Awake()
		{
			// Create texture
			var texture = GenerateTexture();

			// Create sprite
			var sprite = Sprite.Create(texture, new Rect(0, 0, size, size), Vector2.zero);

			// Create SpriteRenderer
			var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

			spriteRenderer.sprite = sprite;

			// Make it destructible
			var destructibleSprite = gameObject.AddComponent<D2dDestructibleSprite>();

			destructibleSprite.Rebuild(2);

			destructibleSprite.ChangeMaterial(); // Upgrade to D2D compatible material

			// Add collider
			var polygonCollider = gameObject.AddComponent<D2dPolygonCollider>();

			polygonCollider.Rebuild();

			// Make splittable
			gameObject.AddComponent<D2dSplitter>();

			// Add Rigidbody2D
			gameObject.AddComponent<Rigidbody2D>();

			// Destroy self
			Destroy(this);
		}

		private Texture2D GenerateTexture()
		{
			var texture = new Texture2D(size, size);
			var center  = new Vector2(size * 0.5f, size * 0.5f);
			var radius  = size * 0.5f - 2.0f;

			for (var y = 0; y < size; y++)
			{
				for (var x = 0; x < size; x++)
				{
					var color = Color.white;
					var point = new Vector2(x, y);

					if (Vector2.Distance(point, center) >= radius)
					{
						color.a = 0.0f;
					}

					texture.SetPixel(x, y, color);
				}
			}

			texture.Apply();

			return texture;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dProceduralSetup;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dProceduralSetup_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("size", "The size of the generated image.");
		}
	}
}
#endif