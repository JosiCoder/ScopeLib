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
using System.Linq;
using System.Collections.Generic;
using ScopeLib.Utilities;

namespace ScopeLib.Sampling
{
    /// <summary>
    /// Provides a base implementation for samplers, i.e. classes that return sample sequence providers
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
        /// Gets the functions that provide the sample sequences, one function per channel.
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


        /// <summary>
        /// Applies the current trigger and aligns all sample sequences accordingly.
        /// </summary>
        /// <param name="externalSampleSequenceProviders">
        /// The functions that provide the raw signal sample sequences, one function per channel.
        /// </param>
        /// <returns>
        /// Functions that provide the signal sample sequences after the trigger has been applied,
        /// one function per channel.
        /// </returns>
        protected IEnumerable<Func<SampleSequence>> ApplyTriggerAndAlignSampleSequences(
            IEnumerable<Func<SampleSequence>> externalSampleSequenceProviders)
        {
            double triggerReferenceTime = 0;

            return externalSampleSequenceProviders.Select((provider, index) =>
            {
                return new Func<SampleSequence>(() =>
                {
                    var sampleSequence = provider();

                    if (index == TriggerChannelIndex)
                        // We are currently providing the sample sequence of the trigger channel, 
                        // determine the trigger reference time.
                    {
                        Trigger.Arm();
                        IEnumerable<double> remaining;
                        var taken = sampleSequence.Values.TakeWhile(element => !Trigger.Check(element), out remaining);
                        sampleSequence.Values = taken.Concat(remaining);
                        var numberOfValuesTakenBeforeTrigger = taken.Count();

                        // TODO: Interpolate considering values before and after trigger. 
                        triggerReferenceTime =
                            Trigger.State == TriggerState.Triggered ?
                            numberOfValuesTakenBeforeTrigger * sampleSequence.TimeIncrement
                            : 0;
                    }

                    // Set the channel's reference time to that of the trigger.
                    sampleSequence.ReferenceTime = triggerReferenceTime;

                    // Buffer the values to ensure that the enumerable provided by the external sample
                    // sequence provider is enumerated just once.
                    // TODO: Skip all values that aren't used.
                    sampleSequence.Values = sampleSequence.Values.ToList();

                    return sampleSequence;
                });
            });
        }
    }
}

