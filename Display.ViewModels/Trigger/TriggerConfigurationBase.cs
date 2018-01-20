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

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides the base implementation for trigger configurations.
    /// </summary>
    public abstract class TriggerConfigurationBase : ViewModelBase
    {
        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        protected TriggerConfigurationBase ()
        {
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="level">The number of the scope channel the trigger is assigned to.</param>
        protected TriggerConfigurationBase (ushort channel)
            : this()
        {
            ChannelNumber = channel;
        }

        /// <summary>
        /// Gets or sets the number of the scope channel the trigger is assigned to.
        /// </summary>
        public ushort ChannelNumber
        { get; set; }


        private double _horizontalPosition;
        /// <summary>
        /// Gets or sets the horizontal position of the trigger point.
        /// </summary>
        public double HorizontalPosition
        {
            get
            {
                return _horizontalPosition;
            }
            set
            {
                _horizontalPosition = value;
                RaisePropertyChanged();
            }
        }
    }
}
