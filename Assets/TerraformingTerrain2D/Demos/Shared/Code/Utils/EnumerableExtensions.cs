using System;
using System.Collections.Generic;

namespace DemosShared
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (T element in list)
            {
                action(element);
            }
        }
    }
}