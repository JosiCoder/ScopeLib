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
    /// Reads a waveform from a waveform file.
    /// </summary>
    public partial class WaveformFileWaveformReader : IDisposable
    {
        /// <summary>
        /// Provides stream of frames read from the waveform file.
        /// </summary>
        private class FrameStream : IFrameStream
        {
            private readonly Stream _stream;
            private readonly WaveformFormat _format;
            private readonly int _frameCount;

            /// <summary>
            /// Initializes an instance of this class.
            /// </summary>
            /// <param name="stream">The stream the frames are read from. The stream's
            /// current position must be right before the frame bytes.</param>
            /// <param name="format">The format of the waveform.</param>
            /// <param name="frameCount">The number of frames within the waveform.</param>
            public FrameStream (Stream stream, WaveformFormat format, int frameCount)
            {
                _stream = stream;
                _format = format;
                _frameCount = frameCount;
            }

            /// <summary>
            /// Returns the frames of the waveform, optionally applying a spacing that returns
            /// every Nth frame only, skipping the frames in between. E.g., a spacing of 4
            /// returns every 4th frame.
            /// </summary>
            /// <param name="frameSpacing">The frame spacing to apply.</param>
            /// <returns>The frames of the waveform.</returns>
            public IEnumerable<IWaveformFrame> GetFrames(int frameSpacing)
            {
                var framesCountLeft = _frameCount;
                while (framesCountLeft >= frameSpacing)
                {
                    SkipFrames(frameSpacing-1);
                    yield return GetNextFrame();

                    framesCountLeft -= frameSpacing;
                }
            }

            /// <summary>
            /// Releases the underlying stream.
            /// </summary>
            public void Dispose()
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                }
            }

            /// <summary>
            /// Skips the specified number of frames.
            /// </summary>
            /// <param name="numberOfFrames">The number of frames to skip.</param>
            private void SkipFrames(int numberOfFrames)
            {
                _stream.SkipBytes(numberOfFrames * _format.FrameSize);
            }

            /// <summary>
            /// Returns the next frame from the underlying stream.
            /// </summary>
            /// <returns>The next frame from the stream.</returns>
            private IWaveformFrame GetNextFrame()
            {
                var frameSampleByteGroups = ReadFrameSampleByteGroups();

                // TODO support more sample wodths
                switch (_format.BitsPerSample)
                {
                    case 16:
                        var frameSamples = frameSampleByteGroups.Select(bg => bg.BytesToInt16());
                        return new WaveForm16BitFrame(frameSamples);
                    default:
                        throw new InvalidDataException(string.Format("Unsupported sample depth ({0} bits/sample).", _format.BitsPerSample));
                }
            }

            /// <summary>
            /// Returns the sample byte groups for the next frame from the stream. Each byte
            /// array returned belongs to a channel of the frame.
            /// </summary>
            /// <returns>The sample byte groups for the next frame from the stream.</returns>
            private IEnumerable<byte[]> ReadFrameSampleByteGroups()
            {
                for (var channelCount = 0; channelCount < _format.ChannelsCount; channelCount++)
                {
                    var streamBytes = _stream.ReadBytes(_format.SampleSize);
                    yield return streamBytes;
                }
            }
        }
    }
}

