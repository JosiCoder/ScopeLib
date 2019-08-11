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
using ScopeLib.Sampling;

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides the viewmodel of a level-based scope trigger.
    /// </summary>
    public class LevelTriggerViewModel : TriggerViewModelBase<LevelTrigger>
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channelVM">
        /// The viewmodel of the scope channel the trigger is assigned to.
        /// </param>
        public LevelTriggerViewModel (ChannelViewModel channelVM)
            : this(new LevelTrigger(), channelVM)
        {
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="channelVM">
        /// The viewmodel of the scope channel the trigger is assigned to.
        /// </param>
        public LevelTriggerViewModel (LevelTrigger trigger, ChannelViewModel channelVM)
            : base(trigger, channelVM)
        {
        }

        /// <summary>
        /// Gets or sets the trigger mode.
        /// </summary>
        public LevelTriggerMode Mode
        {
            get
            {
                return InternalTrigger.Mode;
            }
            set
            {
                InternalTrigger.Mode = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the trigger level.
        /// </summary>
        public double Level
        {
            get
            {
                return InternalTrigger.Level;
            }
            set
            {
                InternalTrigger.Level = value;
                RaisePropertyChanged();
            }
        }
    }
}

