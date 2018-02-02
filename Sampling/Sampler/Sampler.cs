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
using System.Linq;
using System.Collections.Generic;

namespace ScopeLib.Sampling
{
    /// <summary>
    /// Provides a sampler that passes the raw signal samples through after tiggering and time increment adjustments.
    /// </summary>
    public class Sampler : SamplerBase
    {
        private readonly IEnumerable<Func<SampleSequence>> _wrappedSequenceProviders;
        private readonly TriggerBase _trigger;
        private double triggerReferenceTime;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="embeddedSequenceProviders">
        /// The functions that provide the raw signal sample sequences, one function per channel.
        /// </param>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="triggerChannelIndex">The index of the channel to apply the trigger on.</param>
        public Sampler (IEnumerable<Func<SampleSequence>> embeddedSequenceProviders, TriggerBase trigger,
            int triggerChannelIndex)
            : base(trigger, triggerChannelIndex)
        {
            _trigger = trigger;
            _wrappedSequenceProviders = WrapSequenceProviders(embeddedSequenceProviders);
        }

        /// <summary>
        /// Gets the functions that provide the signal sample sequences,
        /// one function per channel.
        /// </summary>
        public override IEnumerable<Func<SampleSequence>> SampleSequenceProviders
        {
            get
            {
                return _wrappedSequenceProviders;
            }
        }

        private IEnumerable<Func<SampleSequence>> WrapSequenceProviders(
            IEnumerable<Func<SampleSequence>> embeddedSequenceProviders)
        {
            return embeddedSequenceProviders.Select((provider, index) =>
            {
                return new Func<SampleSequence>(() =>
                {
                    var sampleSequence = provider();

                    if (index == TriggerChannelIndex)
                    {
                        // We are currently providing the sample sequence of the trigger channel, determine
                        // the reference time.
                        triggerReferenceTime = 0;//TODO
                    }

                    // Set the channel's reference time to that of the trigger.
                    sampleSequence.ReferenceTime = triggerReferenceTime;

                    return sampleSequence;
                });
            });
        }
    }
}
