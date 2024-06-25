using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This class allows you to stamp every destructible object in the scene, or just calculate the stamp matrix for later use.</summary>
	public static class D2dStamp
	{
		/// <summary>This will return the transformation matrix used to convert between world space and stamp sprite space.</summary>
		public static Matrix4x4 CalculateMatrix(Vector2 position, Vector2 size, float angle)
		{
			var t = Matrix4x4.Translate(new Vector3(position.x, position.y, 0.0f));
			var r = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, angle), Vector3.one);
			var s = Matrix4x4.Scale(new Vector3(size.x, size.y, 1.0f));
			var o = Matrix4x4.Translate(new Vector3(-0.5f, -0.5f, 0.0f));

			return t * r * s * o;
		}

		public static void All(D2dDestructible.PaintType paint, Vector2 position, Vector2 size, float angle, D2dShape shape, Color tint, int layerMask = -1, D2dDestructible exclude = null)
		{
			All(paint, CalculateMatrix(position, size, angle), shape, tint, layerMask, exclude);
		}

		public static void All(D2dDestructible.PaintType paint, Matrix4x4 matrix, D2dShape shape, Color tint, int layerMask = -1, D2dDestructible exclude = null)
		{
			var destructible = D2dDestructible.FirstInstance;

			for (var i = D2dDestructible.InstanceCount - 1; i >= 0; i--)
			{
				if (CwHelper.IndexInMask(destructible.gameObject.layer, layerMask) == true && destructible != exclude)
				{
					destructible.Paint(paint, matrix, shape, tint);
				}

				destructible = destructible.NextInstance;
			}
		}
	}
}