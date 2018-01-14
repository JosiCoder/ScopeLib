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
    }
}

