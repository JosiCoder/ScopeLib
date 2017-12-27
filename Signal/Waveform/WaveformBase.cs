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

namespace ScopeLib.Signal
{
    /// <summary>
    /// Provides facilities to access a waveform.
    /// </summary>
    public interface IWaveform
    {
        /// <summary>
        /// Gets the format of the waveform.
        /// </summary>
        WaveformFormat Format { get; }

        /// <summary>
        /// Gets the number of frames within the waveform.
        /// </summary>
        int FrameCount { get; }

        /// <summary>
        /// Returns the frames of the waveform.
        /// </summary>
        /// <returns>The frames of the waveform.</returns>
        IEnumerable<IWaveformFrame> GetFrames();
    }

    /// <summary>
    /// Provides the base functionality for waveforms.
    /// </summary>
    public abstract class WaveformBase : IWaveform
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="format">The format of the waveform.</param>
        public WaveformBase (WaveformFormat format)
        {
            Format = format;
        }

        /// <summary>
        /// Gets the format of the waveform.
        /// </summary>
        public WaveformFormat Format
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of frames within the waveform.
        /// </summary>
        public abstract int FrameCount
        {
            get;
        }

        /// <summary>
        /// Returns the frames of the waveform.
        /// </summary>
        /// <returns>The frames of the waveform.</returns>
        public abstract IEnumerable<IWaveformFrame> GetFrames();
    }
}

