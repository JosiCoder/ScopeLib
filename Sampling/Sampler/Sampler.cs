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
{//TODO: Comments
    /// <summary>
    /// Provides a sample sequence, i.e. a bunch of values sampled from a signal,
    /// and some related meta information.
    /// </summary>
    public class Sampler
    {
//        /// <summary>
//        /// Initializes an instance of this class with default settings.
//        /// </summary>
//        public Sampler ()
//        {
//        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="timeIncrement">The time increment between two successive sampled.</param>
        /// <param name="referenceTime">The time value of the reference point (e.g. the trigger time).</param>
        /// <param name="values">The sample values.</param>
        public Sampler (IEnumerable<Func<SampleSequence>> sampleSequenceProviders)
        {
            SampleSequenceProviders = sampleSequenceProviders;
        }

        /// <summary>
        /// Gets or sets the time increment between two successive measurements.
        /// </summary>
        public IEnumerable<Func<SampleSequence>> SampleSequenceProviders
        { get; private set; }
//
//        /// <summary>
//        /// Gets or sets the time value of the reference point (e.g. the trigger position).
//        /// </summary>
//        public double ReferenceTime
//        { get; set; }
//
//        /// <summary>
//        /// Gets or sets the sample values.
//        /// </summary>
//        public IEnumerable<double> Values
//        { get; set; }
    }
}

