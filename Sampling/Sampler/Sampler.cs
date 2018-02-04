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

namespace ScopeLib.Sampling
{
    /// <summary>
    /// Provides a sampler that uses raw samples provided by external sample sequence provider functions.
    /// </summary>
    public class Sampler : SamplerBase
    {
        private readonly IEnumerable<Func<SampleSequence>> _wrappedSampleSequenceProviders;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="externalSampleSequenceProviders">
        /// The functions that provide the external signal sample sequences, one function per channel.
        /// </param>
        /// <param name="trigger">The trigger to use.</param>
        /// <param name="triggerChannelIndex">The index of the channel to apply the trigger on.</param>
        public Sampler (IEnumerable<Func<SampleSequence>> externalSampleSequenceProviders, TriggerBase trigger,
            int triggerChannelIndex)
            : base(trigger, triggerChannelIndex)
        {
            _wrappedSampleSequenceProviders = ApplyTriggerAndAlignSampleSequences(externalSampleSequenceProviders);
        }

        /// <summary>
        /// Gets the functions that provide the sample sequences, one function per channel.
        /// </summary>
        public override IEnumerable<Func<SampleSequence>> SampleSequenceProviders
        {
            get
            {
                return _wrappedSampleSequenceProviders;
            }
        }
    }
}

