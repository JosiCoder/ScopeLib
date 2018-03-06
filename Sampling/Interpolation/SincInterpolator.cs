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
    /// Provides sinc interpolation for a sequence of sample values.
    /// See https://en.wikipedia.org/wiki/Whittaker%E2%80%93Shannon_interpolation_formula
    /// for more details.
    /// </summary>
    public class SincInterpolator : IInterpolator
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
        /// <param name="originalSampleInterval">
        /// The sample rate of the original sample values.
        /// </param>
        /// <param name="interpolatedSampleInterval">
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
                // Interpolate using an interpolation window that spans across all available original values
                // Ideally, it should be -infinity to +infinity. Thus, we get interpolation artifacts at both
                // ends of the value sequence.
                var value = originalValues
                    .Select ((y, n) => new {y, n})
                    .Sum(x_of_n => x_of_n.y * Sinc((t - x_of_n.n * T) / T));
                list.Add (value);
            }
            return list;
        }

        /// <summary>
        /// Calculates the normalized sinc value of the value specified.
        /// See https://en.wikipedia.org/wiki/Sinc_function for more details.
        /// </summary>
        private double Sinc(double x)
        {
            var nv = Math.PI * x;
            return x == 0f ? 1 : (Math.Sin (nv) / nv);
        }
    }
}

