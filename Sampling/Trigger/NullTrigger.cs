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
    /// Provides a non-fuctional dummy scope trigger.
    /// </summary>
    public class NullTrigger : TriggerBase
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public NullTrigger ()
            : base()
        {
        }

        /// <summary>
        /// Checks the trigger using the current value.
        /// </summary>
        protected override void DoCheck(double value)
        {
            // The dummy trigger triggers immediately on the first value.
            State = TriggerState.Triggered;
        }
    }
}

