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
using System.ComponentModel;
using System.Collections.Generic;
using ScopeLib.Sampling;

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides access to a viewmodel of a scope screen.
    /// </summary>
    public interface IScopeScreenViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the sample sequences have changed.
        /// </summary>
        event EventHandler<EventArgs> SampleSequencesChanged;

        /// <summary>
        /// Gets or sets the graphbase viewmodel.
        /// </summary>
        GraphbaseViewModel GraphbaseVM
        { get; set; }

        /// <summary>
        /// Gets or sets the channel viewmodels, one item per channel.
        /// </summary>
        IEnumerable<ChannelViewModel> ChannelVMs
        { get; set; }

        /// <summary>
        /// Gets or sets the functions that provide the signal sample sequences,
        /// one function per channel.
        /// </summary>
        IEnumerable<Func<SampleSequence>> SampleSequenceProviders
        { get; set; }

        /// <summary>
        /// Gets or sets sample sequences.
        /// </summary>
        IEnumerable<SampleSequence> SampleSequences
        { get; set; }
    }
}

