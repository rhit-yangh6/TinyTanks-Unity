using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This class allows you to slice every destructible object in the scene, or just calculate the slice matrix for later use.</summary>
	public static class D2dSlice
	{
		private static RaycastHit2D[] results = new RaycastHit2D[128];

		/// <summary>This will return the transformation matrix used to convert between world space and slice sprite space.</summary>
		public static Matrix4x4 CalculateMatrix(Vector2 startPos, Vector2 endPos, float thickness)
		{
			var mid   = (startPos + endPos) / 2.0f;
			var vec   = endPos - startPos;
			var size  = new Vector2(thickness, vec.magnitude);
			var angle = CwHelper.Atan2(vec) * -Mathf.Rad2Deg;

			return D2dStamp.CalculateMatrix(mid, size, angle);
		}

		public static void All(D2dDestructible.PaintType paint, Vector2 startPos, Vector2 endPos, float thickness, D2dShape shape, Color tint, int layerMask = -1, D2dDestructible exclude = null)
		{
			D2dStamp.All(paint, CalculateMatrix(startPos, endPos, thickness), shape, tint, layerMask, exclude);
		}

		public static void ForceAll(Vector2 startPos, Vector2 endPos, float thickness, float force, ForceMode2D forceMode, int layerMask = -1)
		{
			var right       = (Vector2)Vector3.Cross(endPos - startPos, Vector3.forward).normalized * thickness * 0.5f;
			var force2      = force * right;
			var resultCount = default(int);

			// Left side
			resultCount = Physics2D.LinecastNonAlloc(startPos - right, endPos - right, results, layerMask);

			Force(resultCount, -force2, forceMode);

			// Right side
			resultCount = Physics2D.LinecastNonAlloc(startPos + right, endPos + right, results, layerMask);

			Force(resultCount, force2, forceMode);
		}

		private static void Force(int resultCount, Vector2 force, ForceMode2D forceMode)
		{
			for (var i = 0; i < resultCount; i++)
			{
				var result = results[i];

				if (result.rigidbody != null)
				{
					result.rigidbody.AddForce(force, forceMode);
				}
			}
		}
	}
}