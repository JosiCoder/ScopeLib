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
    /// Specifies the states available for a scope trigger.
    /// </summary>
    public enum TriggerState : short
    {
        Inactive,
        Armed,
        Triggered,
    }

    /// <summary>
    /// Provides access to a scope trigger.
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// Gets the trigger state.
        /// </summary>
        TriggerState State
        { get; }

        /// <summary>
        /// Arm the trigger, i.e. prepares it to wait for the trigger condition.
        /// </summary>
        void Arm ();

        /// <summary>
        /// Checks the trigger using the current value.
        /// </summary>
        /// <param name="value">The value used to check the trigger.</param>
        /// <returns>
        /// A value indicating whether the trigger has been triggered by the current value or
        /// was already triggered before.
        /// </returns>
        bool Check (double value);
    }

    /// <summary>
    /// Provides the base implementation for scope triggers.
    /// </summary>
    public abstract class TriggerBase : ITrigger
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        protected TriggerBase ()
        {
        }

        /// <summary>
        /// Gets the trigger state.
        /// </summary>
        public TriggerState State
        { get; protected set; }

        /// <summary>
        /// Arm the trigger, i.e. prepares it to wait for the trigger condition.
        /// </summary>
        public virtual void Arm()
        {
            State = TriggerState.Armed;
        }

        /// <summary>
        /// Checks the trigger using the current value.
        /// </summary>
        /// <param name="value">The value used to check the trigger.</param>
        /// <returns>
        /// A value indicating whether the trigger has been triggered by the current value or
        /// was already triggered before.
        /// </returns>
        public bool Check(double value)
        {
            if (State == TriggerState.Triggered)
            {
                return true;
            }

            DoCheck(value);

            return State == TriggerState.Triggered;
        }

        /// <summary>
        /// Checks the trigger using the current value.
        /// </summary>
        protected abstract void DoCheck(double value);
    }
}

