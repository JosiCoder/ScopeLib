﻿//------------------------------------------------------------------------------
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
    /// Provides the configuration of an unspecified trigger.
    /// </summary>
    public class NullTriggerConfiguration : TriggerConfigurationBase<NullTrigger>
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public NullTriggerConfiguration ()
            : this(new NullTrigger(), new ChannelConfiguration())
        {
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="channelConfiguration">
        /// The configuration of the scope channel the trigger is assigned to.
        /// </param>
        public NullTriggerConfiguration (NullTrigger trigger, ChannelConfiguration channelConfiguration)
            : base(trigger, channelConfiguration)
        {
        }
    }
}
