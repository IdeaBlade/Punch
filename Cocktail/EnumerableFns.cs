using System;
using System.Collections.Generic;

namespace Cocktail
{
    public static class EnumerableFns
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var local in items)
            {
                action(local);
            }
        }
    }
}

