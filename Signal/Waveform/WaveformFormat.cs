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

namespace ScopeLib.Signal
{
    /// <summary>
    /// Desribes the format of a waveform.
    /// </summary>
    public class WaveformFormat
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channelsCount">The number of channels.</param>
        /// <param name="samplesPerSecond">The number of samples per second.</param>
        /// <param name="bitsPerSample">The number of bits per sample.</param>
        public WaveformFormat (short channelsCount, int samplesPerSecond, short bitsPerSample)
        {
            var sampleSize = GetSampleSize(bitsPerSample);
            var frameSize = GetFrameSize(channelsCount, sampleSize);
            var bytesPerSecond = GetBytesPerSecond(frameSize, samplesPerSecond);

            ChannelsCount = channelsCount;
            SamplesPerSecond = samplesPerSecond;
            BitsPerSample = bitsPerSample;
        }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public short ChannelsCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of samples per second.
        /// </summary>
        public int SamplesPerSecond
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of bits per sample.
        /// </summary>
        public short BitsPerSample
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of bytes used by a sample.
        /// </summary>
        public short SampleSize
        {
            get
            {
                return GetSampleSize(BitsPerSample);
            }
        }

        /// <summary>
        /// Gets the number of bytes used by a frame.
        /// </summary>
        public short FrameSize
        {
            get
            {
                return GetFrameSize(ChannelsCount, SampleSize);
            }
        }

        /// <summary>
        /// Gets the number of bytes used per second.
        /// </summary>
        public int BytesPerSecond
        {
            get
            {
                return GetBytesPerSecond(FrameSize, SamplesPerSecond);
            }
        }

        /// <summary>
        /// Returns the number of bytes used by a sample using the specified number of bits.
        /// </summary>
        /// <param name="bitsPerSample">The sample size in bits.</param>
        /// <returns>The number of bytes used by the sample.</returns>
        public static short GetSampleSize(short bitsPerSample)
        {
            return (short) ((bitsPerSample + 7) / 8);
        }

        /// <summary>
        /// Gets the number of bytes used by a frame having the specified number of channels
        /// and holding samples of the specified size.
        /// </summary>
        /// <param name="channelsCount">The number of channels.</param>
        /// <param name="sampleSize">The size of each sample.</param>
        /// <returns>The number of bytes used by the frame.</returns>
        public static short GetFrameSize(short channelsCount, short sampleSize)
        {
            return (short)(channelsCount * sampleSize);
        }

        /// <summary>
        /// Gets the number of bytes needed per second for a stream using the
        /// specified frame size and sample rate.
        /// </summary>
        /// <param name="frameSize">The size of each frame.</param>
        /// <param name="samplesPerSecond">The number of samples per second.</param>
        /// <returns>The number of bytes needed per second for the stream.</returns>
        public static int GetBytesPerSecond(short frameSize, int samplesPerSecond)
        {
            return frameSize * samplesPerSecond;
        }
    }
}

