using UnityEngine;

namespace Destructible2D
{
	/// <summary>This class stores a snapshot of a D2dDestructible at one point in time so it can later be reverted to, or gradually faded to.</summary>
	[System.Serializable]
	public class D2dSnapshotData
	{
		public bool      Ready;
		public Color32[] AlphaData;
		public Vector2   AlphaOffset;
		public Vector2   AlphaScale;
		public int       AlphaWidth;
		public int       AlphaHeight;

		/// <summary>This will return a snapshot of the specified D2dDestructible.</summary>
		public static D2dSnapshotData Create(D2dDestructible destructible)
		{
			if (destructible != null && destructible.Ready == true)
			{
				var data = new D2dSnapshotData();

				data.Save(destructible);

				return data;
			}

			return null;
		}

		/// <summary>This will clear all snapshot data.</summary>
		public void Clear()
		{
			Ready     = false;
			AlphaData = null;
		}

		/// <summary>This will store the specified D2dDestructible's state to this snapshot.</summary>
		public void Save(D2dDestructible destructible)
		{
			if (destructible != null && destructible.Ready == true)
			{
				var total = destructible.AlphaWidth * destructible.AlphaHeight;

				if (AlphaData == null || AlphaData.Length != total)
				{
					AlphaData = new Color32[total];
				}

				for (var i = 0; i < total; i++)
				{
					AlphaData[i] = destructible.AlphaData[i];
				}

				Ready       = true;
				AlphaOffset = destructible.AlphaOffset;
				AlphaScale  = destructible.AlphaScale;
				AlphaWidth  = destructible.AlphaWidth;
				AlphaHeight = destructible.AlphaHeight;
			}
			else
			{
				Ready = false;
			}
		}

		/// <summary>This will copy this snapshot to the specified D2dDestructible.</summary>
		public void Load(D2dDestructible destructible)
		{
			if (destructible != null && Ready == true)
			{
			}
		}
	}
}