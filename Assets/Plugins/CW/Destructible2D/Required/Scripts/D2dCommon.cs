using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This class contains some useful methods used by this asset.</summary>
	internal static class D2dCommon
	{
		public const string HelpUrlPrefix = "https://carloswilkes.com/Documentation/Destructible2D#";

		public const string ComponentMenuPrefix = "Destructible 2D/D2D ";

		public static Color32[] tempAlphaData;

		private static Texture2D tempTexture2D;

		public static Color32 Sample(Color32[] data, Vector2Int size, Vector2 coord)
		{
			var x = Mathf.RoundToInt(size.x * coord.x);
			var y = Mathf.RoundToInt(size.y * coord.y);

			x = Mathf.Clamp(x, 0, size.x - 1);
			y = Mathf.Clamp(y, 0, size.y - 1);

			return data[x + y * size.x];
		}

		public static RectInt SpritePixelRect(Sprite sprite)
		{
			if (sprite != null)
			{
				var x = Mathf.FloorToInt(sprite.textureRect.xMin);
				var y = Mathf.FloorToInt(sprite.textureRect.yMin);
				var w = Mathf.CeilToInt(sprite.textureRect.xMax) - x;
				var h = Mathf.CeilToInt(sprite.textureRect.yMax) - y;

				return new RectInt(x, y, w, h);
			}

			return default(RectInt);
		}

		public static void ReserveTempAlphaData(int width, int height)
		{
			if (width <= 0 || height <= 0)
			{
				throw new System.ArgumentOutOfRangeException("Invalid width or height");
			}

			var total = width * height;

			// Replace alpha data array?
			if (tempAlphaData == null || tempAlphaData.Length < total)
			{
				tempAlphaData = new Color32[total];
			}
		}

		public static void ReserveTempAlphaDataClear(int width, int height)
		{
			if (width <= 0 || height <= 0)
			{
				throw new System.ArgumentOutOfRangeException("Invalid width or height");
			}

			var total = width * height;

			// Replace alpha data array?
			if (tempAlphaData == null || tempAlphaData.Length < total)
			{
				tempAlphaData = new Color32[total];
			}
			else
			{
				for (var i = 0; i < total; i++)
				{
					tempAlphaData[i] = default(Color32);
                }
			}
		}

		private static void PrepareTempTexture(int w, int h)
		{
			if (tempTexture2D == null)
			{
				tempTexture2D = new Texture2D(w, h, TextureFormat.ARGB32, false);
			}
			else if (tempTexture2D.width != w || tempTexture2D.height != h)
			{
				tempTexture2D.Reinitialize(w, h, TextureFormat.ARGB32, false);
			}
		}

		public static Color32[] ReadPixels(Texture texture, int x, int y, int w, int h)
		{
			if (texture is Texture2D && texture.isReadable == true && x >= 0 && y >= 0 && (x + w) <= texture.width && (y + h) <= texture.height)
			{
				return ReadPixelsDirect(texture, x, y, w, h);
			}

			var oldActive     = RenderTexture.active;
			var desc          = new RenderTextureDescriptor(texture.width, texture.height, RenderTextureFormat.ARGB32, 0);
			var renderTexture = CwRenderTextureManager.GetTemporary(desc, "D2dCommon ReadPixels"); // TODO: Only blit the rect

			Graphics.Blit(texture, renderTexture);

			RenderTexture.active = renderTexture;

			PrepareTempTexture(w, h);

			if (SystemInfo.graphicsUVStartsAtTop == true && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Metal) // Metal gives the wrong value for graphicsUVStartsAtTop?!
			{
				y = texture.height - y - h;
			}

			tempTexture2D.ReadPixels(new Rect(x, y, w, h), 0, 0);

			RenderTexture.active = oldActive;

			CwRenderTextureManager.ReleaseTemporary(renderTexture);

			return tempTexture2D.GetPixels32();
		}

		public static Color32[] ReadPixelsDirect(Texture texture, int x, int y, int w, int h)
		{
			var pixels    = new Color32[w * h];
			var texture2D = texture as Texture2D;

			if (texture2D != null)
			{
				for (var sampleY = 0; sampleY < h; sampleY++)
				{
					for (var sampleX = 0; sampleX < w; sampleX++)
					{
						pixels[sampleX + sampleY * w] = texture2D.GetPixel(sampleX + x, sampleY + y);
					}
				}
			}

			return pixels;
		}

		public static void PasteAlpha(Color32[] src, int srcWidth, int srcXMin, int srcXMax, int srcYMin, int srcYMax, int dstXMin, int dstYMin, int dstWidth)
		{
			for (var srcY = srcYMin; srcY < srcYMax; srcY++)
			{
				var dstOffset = (srcY - srcYMin + dstYMin) * dstWidth - srcXMin + dstXMin;
				var srcOffset = srcY * srcWidth;

				for (var srcX = srcXMin; srcX < srcXMax; srcX++)
				{
					var dstI = dstOffset + srcX;
					var srcI = srcOffset + srcX;

					tempAlphaData[dstI] = src[srcI];
				}
			}
		}

		public static Vector3 ScreenToWorldPosition(Vector2 screenPosition, float intercept, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			if (camera == null) return screenPosition;

			// Get ray of screen position
			var ray = camera.ScreenPointToRay(screenPosition);

			// Find point along this ray that intersects with Z = 0
			var distance = CwHelper.Divide(ray.origin.z - intercept, ray.direction.z);

			return ray.origin - ray.direction * distance;
		}

		public static bool CalculateRect(Matrix4x4 matrix, ref D2dRect rect, int sizeX, int sizeY)
		{
			// Grab transformed corners
			var a = matrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.0f));
			var b = matrix.MultiplyPoint(new Vector3(1.0f, 0.0f, 0.0f));
			var c = matrix.MultiplyPoint(new Vector3(0.0f, 1.0f, 0.0f));
			var d = matrix.MultiplyPoint(new Vector3(1.0f, 1.0f, 0.0f));
			
			// Find min/max x/y
			var minX = Mathf.Min(Mathf.Min(a.x, b.x), Mathf.Min(c.x, d.x));
			var maxX = Mathf.Max(Mathf.Max(a.x, b.x), Mathf.Max(c.x, d.x));
			var minY = Mathf.Min(Mathf.Min(a.y, b.y), Mathf.Min(c.y, d.y));
			var maxY = Mathf.Max(Mathf.Max(a.y, b.y), Mathf.Max(c.y, d.y));
			
			// Has volume?
			if (minX < maxX && minY < maxY)
			{
				rect.MinX = Mathf.FloorToInt(minX * sizeX);
				rect.MaxX = Mathf. CeilToInt(maxX * sizeX);
				rect.MinY = Mathf.FloorToInt(minY * sizeY);
				rect.MaxY = Mathf. CeilToInt(maxY * sizeY);
				
				return true;
			}

			return false;
		}
	}
}