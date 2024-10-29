using System;
using System.Collections.Generic;
using System.Linq;

namespace GravityLib.Common.Extensions;

/// <summary>
/// Helper methods for easier manipulation of enumerable collections
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>Performs the specified action on each element of the <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
    /// <param name="items">The enumerable collection to process</param>
    /// <param name="action">The <see cref="T:System.Action`1" /> delegate to perform on each element of the <see cref="T:System.Collections.Generic.IEnumerable`1" />.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="items" /> is null.</exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="action" /> is null.</exception>
    /// <returns>The input collection, so that the method can be chained</returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        foreach (var item in items)
        {
            action(item);
        }
        return items;
    }

    /// <summary>
    /// Returns the enumerable as chunks of lists of the objects, split by the chunk size
    /// </summary>
    public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    /// <summary>
    /// Enumerates and distincts by the given lambda
    /// </summary>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}