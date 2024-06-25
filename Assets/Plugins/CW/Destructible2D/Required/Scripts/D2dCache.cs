using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	/// <summary>This class handles various ways to cache values to improve performance.</summary>
	public static class D2dCache
	{
		/// <summary>This class pools T arrays to avoid runtime gc-alloc, because some Unity properties only accept arrays that need to change in size between assignments.</summary>
		public static class CachedList<T>
		{
			class Entry
			{
				public int Used;

				public List<T[]> Instances = new List<T[]>();

				public T[] GetNext(int size)
				{
					if (Used == Instances.Count)
					{
						Instances.Add(new T[size]);
					}

					return Instances[Used++];
				}
			}

			private static List<Entry> entries = new List<Entry>();

			/// <summary>Call this before using any of the returned arrays.</summary>
			public static void Clear()
			{
				for (var i = entries.Count - 1; i >= 0; i--)
				{
					entries[i].Used = 0;
				}
			}

			/// <summary>This will return a cached array copy of the specified list, and can be called multiple times with the same list length.</summary>
			public static T[] ToArray(List<T> list)
			{
				var size  = list.Count;
				var count = size + 1;

				for (var i = entries.Count; i < count; i++)
				{
					entries.Add(new Entry());
				}

				var array = entries[list.Count].GetNext(size);

				for (var i = list.Count - 1; i >= 0; i--)
				{
					array[i] = list[i];
				}

				return array;
			}
		}
	}
}