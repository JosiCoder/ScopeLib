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
    /// Represents a waveform read from a stream of frames.
    /// </summary>
    public class StreamWaveform : WaveformBase, IDisposable
    {
        private readonly int _frameCount;
        private readonly IFrameStream _frameStream;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="format">The format of the waveform.</param>
        /// <param name="frameCount">The number of frames within the waveform.</param>
        /// <param name="frameStream">The stream providing the waveform's frames.</param>
        public StreamWaveform (WaveformFormat format, int frameCount, IFrameStream frameStream)
            : base(format)
        {
            _frameCount = frameCount;
            _frameStream = frameStream;
        }

        /// <summary>
        /// Gets the number of frames within the waveform.
        /// </summary>
        public override int FrameCount
        {
            get { return _frameCount; }
        }

        /// <summary>
        /// Returns the frames of the waveform.
        /// </summary>
        /// <returns>The frames of the waveform.</returns>
        public override IEnumerable<IWaveformFrame> GetFrames()
        {
            return GetFrames(1);
        }

        /// <summary>
        /// Returns the frames of the waveform, applying a spacing that returns every
        /// Nth frame only, skipping the frames in between. E.g., a spacing of 4
        /// returns every 4th frame.
        /// </summary>
        /// <param name="frameSpacing">The frame spacing to apply.</param>
        /// <returns>The frames of the waveform.</returns>
        public IEnumerable<IWaveformFrame> GetFrames(int frameSpacing)
        {
            return _frameStream.GetFrames(frameSpacing);
        }

        /// <summary>
        /// Releases the underlying frame source.
        /// </summary>
        public void Dispose()
        {
            if (_frameStream != null)
            {
                _frameStream.Dispose();
            }
        }
    }
}

