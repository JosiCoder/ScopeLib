using System;
using System.Collections;
using System.Collections.Generic;

namespace ScopeLib.Utilities
{
    /// <summary>
    /// Provides an enumerable that caches the items obtained from the enumerator passed to the
    /// constructor. This prevents the original enumerator from beeing enumerated multiple times.
    /// </summary>
    /// <typeparam name="T">The type of the items cached and forwarded.</typeparam>
    public class CachedEnumerable<T> : IEnumerable<T>
    {
        readonly List<T> _itemCache;
        IEnumerator<T> _wrappedEnumerator;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="wrappedEnumerator">
        /// The enumerator used to obtain the items cached and forwarded.
        /// </param>
        public CachedEnumerable (IEnumerator<T> wrappedEnumerator)
        {
            _itemCache = new List<T>();
            _wrappedEnumerator = wrappedEnumerator;
        }

        /// <summary>
        /// Gets an an enumerator to access the cached items
        /// </summary>
        /// <returns>An enumerator to access the cached items.</returns>
        public IEnumerator<T> GetEnumerator ()
        {
            // As long as there are cached items available, yield them.
            foreach (var item in _itemCache)
            {
                yield return item;
            }

            // Get the remaining items from the wrapped enumerator, cache and yield them.
            if (_wrappedEnumerator != null)
            {
                while (_wrappedEnumerator.MoveNext())
                {
                    var item = _wrappedEnumerator.Current;
                    _itemCache.Add (item);
                    yield return item;
                }

                // The wrapped enumerator is exhausted, terminate it.
                _wrappedEnumerator.Dispose();
                _wrappedEnumerator = null;
            }
        }

        /// <summary>
        /// Gets an an enumerator to access the cached items
        /// </summary>
        /// <returns>An enumerator to access the cached items.</returns>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Provides some utilities for cached enumarables.
    /// </summary>
    public static class CachedEnumerableExtensions
    {
        /// <summary>
        /// Returns a cached enumerable for the specified enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items cached and forwarded.</typeparam>
        /// <param name="enumerable">The enumerable to create a cached enumerable for.</param>
        /// <returns>The cached enumerable created.</returns>
        public static CachedEnumerable<T> ToCachedEnumerable<T> (this IEnumerable<T> enumerable)
        {
            var result = (enumerable as CachedEnumerable<T>)
                ?? new CachedEnumerable<T>(enumerable.GetEnumerator());
            return result;
        }
    }
}
