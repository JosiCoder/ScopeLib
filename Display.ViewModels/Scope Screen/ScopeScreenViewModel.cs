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
using ScopeLib.Sampling;

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides the viewmodel of a scope screen.
    /// </summary>
    public class ScopeScreenViewModel : ViewModelBase, IScopeScreenViewModel
    {
        /// <summary>
        /// Occurs when the sample sequences have been refreshed.
        /// </summary>
        public event EventHandler<EventArgs> SampleSequencesRefreshed;

        /// <summary>
        /// Gets or sets the graphbase viewmodel.
        /// </summary>
        public GraphbaseViewModel GraphbaseVM
        { get; set; }

        /// <summary>
        /// Gets or sets the channel viewmodels, one item per channel.
        /// </summary>
        public IEnumerable<ChannelViewModel> ChannelVMs
        { get; set; }

        /// <summary>
        /// Gets or sets the functions that provide the signal sample sequences,
        /// one function per channel.
        /// </summary>
        public IEnumerable<Func<SampleSequence>> SampleSequenceProviders
        { get; set; }

        /// <summary>
        /// Refreshes (i.e. re-enumerates) the sample sequences.
        /// </summary>
        public void RefreshSampleSequences()
        {
            SampleSequencesRefreshed.Raise(this, EventArgs.Empty);
        }
    }
}
