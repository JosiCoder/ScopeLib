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

namespace ScopeLib.Sampling
{
    /// <summary>
    /// Provides access to an interpolator.
    /// </summary>
    public interface IInterpolator
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
        IEnumerable<double> Interpolate(IEnumerable<double> originalValues,
            double startTime, double endTime,
            double originalSampleRate, double interpolatedSampleRate);
    }
}

