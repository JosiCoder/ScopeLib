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
using System.IO;

namespace ScopeLib.Signal
{
    /// <summary>
    /// Represents a waveform stored in memory.
    /// </summary>
    public class MemoryWaveform : WaveformBase
    {
        private readonly IEnumerable<IWaveformFrame> _frames;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="format">The format of the waveform.</param>
        /// <param name="frames">The frames within the waveform.</param>
        public MemoryWaveform (WaveformFormat format, IEnumerable<IWaveformFrame> frames)
            : base(format)
        {
            _frames = frames;
        }

        /// <summary>
        /// Gets the number of frames within the waveform.
        /// </summary>
        public override int FrameCount
        {
            get { return _frames.Count(); }
        }

        /// <summary>
        /// Returns the frames of the waveform.
        /// </summary>
        /// <returns>The frames of the waveform.</returns>
        public override IEnumerable<IWaveformFrame> GetFrames()
        {
            return _frames;
        }
    }
}

