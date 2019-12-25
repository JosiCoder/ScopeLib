//------------------------------------------------------------------------------
// Copyright (C) 2016 Josi Coder

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
using ScopeLib.Sampling;

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides data for the SampleSequencesRefreshed event.
    /// </summary>
    public class SampleSequencesRefreshedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="sampleSequences">The new sample sequences.</param>
        public SampleSequencesRefreshedEventArgs(IEnumerable<SampleSequence> sampleSequences)
        {
            SampleSequences = sampleSequences;
        }

        /// <summary>
        /// Gets or sets the new sample sequences.
        /// </summary>
        public IEnumerable<SampleSequence> SampleSequences;
    }
}