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
using System.Collections.Generic;

namespace ScopeLib.Utilities
{
    /// <summary>
    /// Generates values for certain functions.
    /// </summary>
    public static class FunctionValueGenerator
    {
        /// <summary>
        /// Generates the sine values for the specified frequency.
        /// </summary>
        /// <typeparam name="TPoint">
        /// The type of the objects representing the values of each iteration.
        /// </typeparam>
        /// <param name="frequency">The frequency to generate sine values for.</param>
        /// <param name="samplesPerSecond">The number of samples per second to generate sine values for.</param>
        /// <param name="durationInSeconds">
        /// The duration to generate sine values for.
        /// </param>
        /// <param name="pointGenerator">
        /// The function that generates the object representing the values of a single iteration.
        /// </param>
        /// <returns>The function values.</returns>
        public static IEnumerable<TPoint> GenerateSineValuesForFrequency<TPoint>(double frequency,
            int samplesPerSecond, double durationInSeconds, Func<double, double, TPoint> pointGenerator)
        {
            var phaseIncrement = 2 * Math.PI * frequency / samplesPerSecond;

            return GenerateSineValuesForAngles(0, samplesPerSecond * durationInSeconds * phaseIncrement,
                phaseIncrement, pointGenerator);
        }

        /// <summary>
        /// Generates the sine values for the specified angles.
        /// </summary>
        /// <typeparam name="TPoint">
        /// The type of the objects representing the values of each iteration.
        /// </typeparam>
        /// <param name="startAngle">The start angle to use (in radians).</param>
        /// <param name="endAngle">The end angle to use (in radians).</param>
        /// <param name="angleIncrement">
        /// The value by which to increment the angle in each iteration (in radians).
        /// </param>
        /// <param name="pointGenerator">
        /// The function that generates the object representing the values of a single iteration.
        /// </param>
        /// <returns>The function values.</returns>
        public static IEnumerable<TPoint> GenerateSineValuesForAngles<TPoint>(double startAngle, double endAngle,
            double angleIncrement, Func<double, double, TPoint> pointGenerator)
        {
            for (var x = startAngle; x <= endAngle; x += angleIncrement)
            {
                yield return pointGenerator(x, Math.Sin (x));
            }
        }
    }
}

