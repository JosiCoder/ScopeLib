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
using System.ComponentModel;
using ScopeLib.Sampling;

namespace ScopeLib.Display.ViewModels
{
    public interface ITriggerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the configuration of the scope channel the trigger is assigned to.
        /// </summary>
        ChannelConfiguration ChannelConfiguration
        { get; set; }

        /// <summary>
        /// Gets or sets the horizontal position of the trigger point.
        /// </summary>
        double HorizontalPosition
        { get; set; }
    }

    /// <summary>
    /// Provides the base implementation for trigger configurations.
    /// </summary>
    /// <typeparam name="TTrigger">The type of the trigger.</typeparam>
    public abstract class TriggerConfigurationBase<TTrigger> : ViewModelBase, ITriggerViewModel
        where TTrigger : TriggerBase
    {
        protected readonly TTrigger Trigger;

        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="channelConfiguration">
        /// The configuration of the scope channel the trigger is assigned to.
        /// </param>
        protected TriggerConfigurationBase (TTrigger trigger, ChannelConfiguration channelConfiguration)
        {
            Trigger = trigger;
            ChannelConfiguration = channelConfiguration;
        }

        /// <summary>
        /// Gets or sets the configuration of the scope channel the trigger is assigned to.
        /// </summary>
        public ChannelConfiguration ChannelConfiguration
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

