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
using ScopeLib.Utilities;

namespace ScopeLib.Sampling
{
    /// <summary>
    /// Provides a bunch of values belonging to equally spaced reference values,
    /// The values can belong to any domain (e.g. time or frequency).
    /// </summary>
    public class SampleSequence
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="sampleInterval">The increment along the X axis between two successive samples.</param>
        /// <param name="values">
        /// The sample values. They are cached to ensure that they aren't accessed multiple times.
        /// </param>
        public SampleSequence (double sampleInterval, IEnumerable<double> values)
        {
            SampleInterval = sampleInterval;
            Values = values.ToCachedEnumerable();
        }

        /// <summary>
        /// Gets or sets the increment along the X axis between two successive samples.
        /// </summary>
        public double SampleInterval
        { get; set; }

        /// <summary>
        /// Gets the sample rate, i.e. the reverse of the sample interval.
        /// </summary>
        public double SampleRate
        {
            get { return 1f / SampleInterval; }
            set { SampleInterval = 1f / value; }
        }

        /// <summary>
        /// Gets or sets the sample values.
        /// </summary>
        public IEnumerable<double> Values
        { get; set; }

        /// <summary>
        /// Gets or sets the reference X value (e.g. the trigger position for time domain samples).
        /// </summary>
        public double ReferenceX
        { get; set; }
    }
}

