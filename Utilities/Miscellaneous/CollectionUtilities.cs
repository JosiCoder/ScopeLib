//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.

// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.

// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;

namespace ScopeLib.Utilities
{
    /// <summary>
    /// Provides some utilities for collections.
    /// </summary>
    public static class CollectionUtilities
    {
        /// <summary>
        /// Combines multiple enumerables of objects into a single enumerable of
        /// combined objects.
        /// </summary>
        /// <typeparam name="TResult">The type of the combined objects.</typeparam>
        /// <param name="resultFactory">The function used create a combined object.</param>
        /// <param name="enumerables">The enumerables holding the objects to combine.</param>
        /// <returns>An enumerable holding the combined objects.</returns>
        public static IEnumerable<TResult> Zip<TResult>(
            Func<object[], TResult> resultFactory,
            params System.Collections.IEnumerable[] enumerables)
        {
            var enumerators = enumerables.Select(i => i.GetEnumerator()).ToArray();

            Func<bool> moveNext = () =>
            {
                foreach (var enumerator in enumerators)
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                }
                return true;
            };

            while (moveNext())
            {
                yield return resultFactory(enumerators.Select(e => e.Current).ToArray());
            }
        }

        /// <summary>
        /// Enumerates an IEnumerable and performs an action on each element.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable elements.</typeparam>
        /// <param name="enumerable">The enumerable to enumerate.</param>
        /// <param name="action">The action to perform on each element.</param>
        public static void ForEachDo<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach(T item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Enumerates an IEnumerable and performs an action on each element. Passes all elements
        /// to the caller in a new enumerable. Processing of each element is deferred until the
        /// caller fetches that element via the enumerable returned.
        /// This method can be used to avoid fetching unnecessary elements or enumerating the original
        /// enumerable multiple times.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable elements.</typeparam>
        /// <param name="enumerable">The enumerable to enumerate.</param>
        /// <param name="action">The action to perform on each element.</param>
        /// <returns>
        /// A new enumerable of all elements.
        /// </returns>
        public static IEnumerable<T> ForEachDoDeferred<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach(T item in enumerable)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Takes the elements of an enumerable as long as the predicate is true.
        /// Returns the taken elements as well as the remaining elements. The remaining
        /// elements except the first one are subject to deferred execution.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable elements.</typeparam>
        /// <param name="enumerable">The enumerable to take elements from.</param>
        /// <param name="predicate">The predicate checked for each element.</param>
        /// <param name="remainingElements">
        /// The remaining elements (i.e. the elements not taken).
        /// </param>
        /// <returns>
        /// The elements taken (i.e. the elements before the first element where the
        /// predicate was wrong.
        /// </returns>
        public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate,
            out IEnumerable<T> remainingElements)
        {
            var takenElementsList = new List<T>();
            remainingElements = new T[0];

            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    takenElementsList.Add(enumerator.Current);
                }
                else
                {
                    remainingElements = enumerator.ToEnumerable(true);
                    break;
                }
            }

            return takenElementsList;
        }

        /// <summary>
        /// Returns a new enumerable that provides the remaining elements (optionaly including the
        /// current one) of the specified enumerator.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable elements.</typeparam>
        /// <param name="enumerator">The enumerator to return an enumerable for.</param>
        /// <param name="includeCurrentElement">
        /// A value indicating whether to include the enumerator's current value.
        /// </param>
        /// <returns>The enumerable provides the remaining elements of the enumerator.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator,
            bool includeCurrentElement)
        {
            if (includeCurrentElement)
            {
                includeCurrentElement = false;
                yield return enumerator.Current;
            }
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }
}

