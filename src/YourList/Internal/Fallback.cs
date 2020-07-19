using System;
using System.Collections.Generic;
using System.Linq;

namespace YourList.Internal
{
    internal static class Fallback
    {
        public static IEnumerable<T> ToEmpty<T>(IEnumerable<T>? maybeSequence)
        {
            return maybeSequence ?? Enumerable.Empty<T>();
        }

        public static IReadOnlyList<T> ToEmpty<T>(IReadOnlyList<T>? maybeList)
        {
            return maybeList ?? Array.Empty<T>();
        }
    }
}