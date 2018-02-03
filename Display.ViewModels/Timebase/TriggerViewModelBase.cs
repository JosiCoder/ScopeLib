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
    /// <summary>
    /// Provides access to a viewmodel of a trigger.
    /// </summary>
    public interface ITriggerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the viewmodel of the scope channel the trigger is assigned to.
        /// </summary>
        ChannelViewModel ChannelVM
        { get; set; }

        /// <summary>
        /// Gets or sets the horizontal position of the trigger point.
        /// </summary>
        double HorizontalPosition
        { get; set; }
    }

    /// <summary>
    /// Provides the base implementation for trigger viewmodels.
    /// </summary>
    /// <typeparam name="TTrigger">The type of the trigger.</typeparam>
    public abstract class TriggerViewModelBase<TTrigger> : ViewModelBase, ITriggerViewModel
        where TTrigger : TriggerBase
    {
        protected readonly TTrigger Trigger;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="channelVM">
        /// The viewmodel of the scope channel the trigger is assigned to.
        /// </param>
        protected TriggerViewModelBase (TTrigger trigger, ChannelViewModel channelVM)
        {
            Trigger = trigger;
            ChannelVM = channelVM;
        }

        /// <summary>
        /// Gets or sets the viewmodel of the scope channel the trigger is assigned to.
        /// </summary>
        public ChannelViewModel ChannelVM
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

