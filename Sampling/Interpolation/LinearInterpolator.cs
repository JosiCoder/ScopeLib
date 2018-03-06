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

namespace ScopeLib.Sampling
{
    /// <summary>
    /// Provides linear interpolation for a sequence of sample values. This is done by
    /// calculating the weighted average of the neighboring sample values of each target
    /// position.
    /// See http://cdn.teledynelecroy.com/files/whitepapers/wp_interpolation_102203.pdf
    /// for more details.
    /// </summary>
    public class LinearInterpolator : IInterpolator
    {
        /// <summary>
        /// Interpolates the specified original values.
        /// </summary>
        /// <param name="originalValues">The original sample values.</param>
        /// <param name="startTime">
        /// The point in time to return the first interpolated value for.
        /// </param>
        /// <param name="endTime">
        /// The point in time to return the last interpolated value for.
        /// </param>
        /// <param name="originalSampleRate">
        /// The sample rate of the original sample values.
        /// </param>
        /// <param name="interpolatedSampleRate">
        /// The sample rate of the interpolated sample values returned.
        /// </param>
        /// <returns>The interpolated sample values.</returns>
        public IEnumerable<double> Interpolate(IEnumerable<double> originalValues,
            double startTime, double endTime,
            double originalSampleRate, double interpolatedSampleRate)
        {
            var T = 1f/originalSampleRate;
            var list = new List<double> ();

            // For each interpolated value to create.
            for (var t = startTime; t <= endTime; t += 1f/interpolatedSampleRate)
            {
                // Use a triangular interpolation window in the closed interval from t-T to t+T. Get the
                // values within that window (2 or 3 values are expected).
                var windowValues = originalValues
                    .Select ((y, n) => new {y, t = n*T})
                    .Where(x_of_t => x_of_t.t >= (t-T) && x_of_t.t <= (t+T))
                    .ToArray();

                // If there are three values within the window, just take the center value as it corresponds to t.
                // If there are two values within the window, take the average of both values, inversely weighted
                // by their normalized distance to t.
                var value =
                    windowValues.Length == 3 ? windowValues[1].y
                    : windowValues.Length == 2 ? windowValues.Sum(x_of_t => x_of_t.y * (1-Math.Abs(t-x_of_t.t)/T))
                    : 0d;

                list.Add (value);
            }
            return list;
        }
    }
}

