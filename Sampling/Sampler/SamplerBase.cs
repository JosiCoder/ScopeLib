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
    /// Provides a base implementation for samplers, i.e. classes that returns sample sequence providers
    /// based on certain sample sources. A sample sequence provider is a function that provides a sequence
    /// of values sampled from a signal.
    /// </summary>
    public abstract class SamplerBase
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="triggerChannelIndex">The index of the channel to apply the trigger on.</param>
        public SamplerBase (TriggerBase trigger, int triggerChannelIndex)
        {
            Trigger = trigger;
            TriggerChannelIndex = triggerChannelIndex;
        }

        /// <summary>
        /// Gets the functions that provide the signal sample sequences,
        /// one function per channel.
        /// </summary>
        public abstract IEnumerable<Func<SampleSequence>> SampleSequenceProviders
        { get; }

        /// <summary>
        /// Gets the trigger to use.
        /// </summary>
        public TriggerBase Trigger
        { get; private set; }

        /// <summary>
        /// Gets the index of the channel to apply the trigger on.
        /// </summary>
        public int TriggerChannelIndex
        { get; private set; }
    }
}

