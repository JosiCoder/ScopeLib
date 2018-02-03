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
{
    /// <summary>
    /// Specifies the modes available for a level-based scope trigger.
    /// </summary>
    public enum LevelTriggerMode : short
    {
        RisingEdge,
        FallingEdge,
    }

    /// <summary>
    /// Provides a level-based scope trigger.
    /// </summary>
    public class LevelTrigger : TriggerBase
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public LevelTrigger ()
            : base()
        {
            Mode = LevelTriggerMode.RisingEdge;
            Level = 0.0;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="mode">The trigger mode.</param>
        /// <param name="level">The trigger level.</param>
        public LevelTrigger (LevelTriggerMode mode, double level)
            : base()
        {
            Mode = mode;
            Level = level;
        }

        /// <summary>
        /// Gets or sets the trigger mode.
        /// </summary>
        public LevelTriggerMode Mode
        { get; set; }

        /// <summary>
        /// Gets or sets the trigger level.
        /// </summary>
        public double Level
        { get; set; }
    }
}

