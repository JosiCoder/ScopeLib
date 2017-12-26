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

namespace ScopeLib.Signal
{
    /// <summary>
    /// Provides facilities to access a waveform frame.
    /// </summary>
    public interface IWaveformFrame
    {
        /// <summary>
        /// Gets the normalized sample values, i.e. within a range of -1.0 to +1.0.
        /// </summary>
        IEnumerable<double> NormalizedSamples
        { get; }
    }

    /// <summary>
    /// Represents a waveform frame based on 16-bit sample values.
    /// </summary>
    public class WaveForm16BitFrame : IWaveformFrame
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="samples">
        /// The sample values to assign to the frame, one value per channel and
        /// each value within the range of a short (i.e. any signed 16-bit value).
        /// </param>
        public WaveForm16BitFrame (IEnumerable<short> samples)
        {
            Samples = samples.ToArray();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="samples">
        /// The sample values to assign to the frame, one value per channel and
        /// each value normalized, i.e. within a range of -1.0 to +1.0.
        /// </param>
        public WaveForm16BitFrame (IEnumerable<double> samples)
        {
            Samples = ToNativeSamples(samples).ToArray();
        }

        /// <summary>
        /// Gets the sample values within the range of a short (i.e. any signed 16-bit value).
        /// </summary>
        public IEnumerable<short> Samples
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the normalized sample values, i.e. within a range of -1.0 to +1.0.
        /// </summary>
        public IEnumerable<double> NormalizedSamples
        {
            get
            {
                return ToNormalizedSamples(Samples);
            }
        }

        /// <summary>
        /// Converts native to normalized sample values
        /// </summary>
        private IEnumerable<double> ToNormalizedSamples(IEnumerable<short> samples)
        {
            return samples.Select(fs => ((double)fs) / short.MaxValue);
        }

        /// <summary>
        /// Converts normalized to native sample values
        /// </summary>
        private IEnumerable<short> ToNativeSamples(IEnumerable<double> samples)
        {
            return samples.Select(fs => (short)(fs * short.MaxValue));
        }
    }

}

