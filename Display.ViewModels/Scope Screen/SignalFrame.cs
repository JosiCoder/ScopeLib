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

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides a signal frame, i.e. a bunch of values captured from a signal including some
    /// meta information.
    /// </summary>
    public class SignalFrame
    {
        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        public SignalFrame ()
        {
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="timeIncrement">The time increment between two successive measurements.</param>
        /// <param name="referenceTime">The time value of the reference point (e.g. the trigger position).</param>
        /// <param name="values">The measurement values.</param>
        public SignalFrame (double timeIncrement, double referenceTime, IEnumerable<double> values)
        {
            TimeIncrement = timeIncrement;
            ReferenceTime = referenceTime;
            Values = values;
        }

        /// <summary>
        /// Gets or sets the time increment between two successive measurements.
        /// </summary>
        public double TimeIncrement
        { get; set; }

        /// <summary>
        /// Gets or sets the time value of the reference point (e.g. the trigger position).
        /// </summary>
        public double ReferenceTime
        { get; set; }

        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        public IEnumerable<double> Values
        { get; set; }
    }
}

