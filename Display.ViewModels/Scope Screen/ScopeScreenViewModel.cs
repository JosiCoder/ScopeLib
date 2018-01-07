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
    /// Provides the viewmodel of a scope screen.
    /// </summary>
    public class ScopeScreenViewModel : ViewModelBase, IScopeScreenViewModel
    {
        /// <summary>
        /// Gets or sets the channel configuration, one item per channel.
        /// </summary>
        public IEnumerable<ChannelConfiguration> ChannelConfigurations
        { get; set; }

        /// <summary>
        /// Gets or sets the current signal frames, one item per channel.
        /// </summary>
        public IEnumerable<SignalFrame> CurrentSignalFrames
        { get; set; }
    }
}

