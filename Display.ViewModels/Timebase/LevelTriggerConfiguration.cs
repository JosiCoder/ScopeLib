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
    /// Provides the configuration of a level-based scope trigger.
    /// </summary>
    public class LevelTriggerConfiguration : TriggerConfigurationBase<LevelTrigger>
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channelConfiguration">
        /// The configuration of the scope channel the trigger is assigned to.
        /// </param>
        public LevelTriggerConfiguration (ChannelConfiguration channelConfiguration)
            : this(new LevelTrigger(), channelConfiguration)
        {
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="channelConfiguration">
        /// The configuration of the scope channel the trigger is assigned to.
        /// </param>
        public LevelTriggerConfiguration (LevelTrigger trigger, ChannelConfiguration channelConfiguration)
            : base(trigger, channelConfiguration)
        {
        }

        /// <summary>
        /// Gets or sets the trigger mode.
        /// </summary>
        public LevelTriggerMode Mode
        {
            get
            {
                return Trigger.Mode;
            }
            set
            {
                Trigger.Mode = value;
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
                return Trigger.Level;
            }
            set
            {
                Trigger.Level = value;
                RaisePropertyChanged();
            }
        }
    }
}

