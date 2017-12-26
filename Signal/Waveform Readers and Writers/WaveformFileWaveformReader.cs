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
    /// Provides facilities to access a stream of frames.
    /// </summary>
    public interface IFrameStream : IDisposable
    {
        /// <summary>
        /// Returns the frames of the waveform, optionally applying a spacing that returns
        /// every Nth frame only, skipping the frames in between. E.g., a spacing of 4
        /// returns every 4th frame.
        /// </summary>
        /// <param name="frameSpacing">The frame spacing to apply.</param>
        /// <returns>The frames of the waveform.</returns>
        IEnumerable<IWaveformFrame> GetFrames (int frameSpacing);
    }

    /// <summary>
    /// Reads a waveform from a waveform file.
    /// </summary>
    public partial class WaveformFileWaveformReader : IDisposable
    {
        private readonly Stream _stream;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="stream">A stream providing the waveform file contents.</param>
        public WaveformFileWaveformReader (Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="path">The path of the waveform file.</param>
        public WaveformFileWaveformReader (string path)
            : this(new FileStream(path, FileMode.Open, FileAccess.Read))
        {
        }

        /// <summary>
        /// Reads the waveform from the underlying stream and returns a streaming waveform.
        /// The underlying stream remains open until the object returned here is disposed.
        /// </summary>
        /// <returns>A streaming waveform that should be disposed after use.</returns>
        public StreamWaveform Read()
        {
            ReadRootChunkPrologue();
            var waveFormFormat = ReadFormatChunk();
            var waveForm = ReadDataChunk(waveFormFormat);
            return waveForm;
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
        /// Reads the prologue of the root chunk from the underlying stream, i.e. all
        /// bytes up to the first nested chunk.
        /// </summary>
        private void ReadRootChunkPrologue()
        {
            ReadChunkHeader(WaveformFileFormat.RootChunkId);
            if (_stream.ReadAsString(4) != WaveformFileFormat.RiffType)
            {
                throw new InvalidDataException("RIFF type wrong.");
            }
        }

        /// <summary>
        /// Reads the chunk describing the waveform format from the underlying stream.
        /// </summary>
        /// <returns>The waveform format.</returns>
        private WaveformFormat ReadFormatChunk()
        {
            var chunkHeader = ReadChunkHeader(WaveformFileFormat.FormatChunkId);
            if (chunkHeader.PayloadSize != WaveformFileFormat.FormatChunkPayloadSize)
            {
                throw new InvalidDataException("Unexpected format chunk size.");
            }

            var formatTag = _stream.ReadAsInt16();
            if (formatTag != WaveformFileFormat.PcmFormatTag)
            {
                throw new InvalidDataException("Unsupported format.");
            }

            var channelsCount = _stream.ReadAsInt16();
            var samplesPerSecond = _stream.ReadAsInt32();
            var bytesPerSecond = _stream.ReadAsInt32();
            var frameSize = _stream.ReadAsInt16();
            var bitsPerSample = _stream.ReadAsInt16();

            var sampleSize = WaveformFormat.GetSampleSize(bitsPerSample);
            var expectedFrameSize = WaveformFormat.GetFrameSize(channelsCount, sampleSize);
            var expectedBytesPerSecond = WaveformFormat.GetBytesPerSecond(frameSize, samplesPerSecond);

            if (frameSize != expectedFrameSize ||
                bytesPerSecond != expectedBytesPerSecond)
            {
                throw new InvalidDataException("Inconsistent sizes specified.");
            }

            return new WaveformFormat(channelsCount, samplesPerSecond, bitsPerSample);
        }

        /// <summary>
        /// Reads the chunk containing the waveform data from the underlying stream and returns
        /// a streaming waveform. The underlying stream remains open until the object returned
        /// here is disposed.
        /// </summary>
        /// <param name="format">The waveform format.</param>
        /// <returns>A streaming waveform that should be disposed after use.</returns>
        private StreamWaveform ReadDataChunk(WaveformFormat format)
        {
            var chunkHeader = ReadChunkHeader(WaveformFileFormat.DataChunkId);
            var frameCount = chunkHeader.PayloadSize / format.FrameSize;

            var frameStream = new FrameStream(_stream, format, frameCount);
            return new StreamWaveform(format, frameCount, frameStream);
        }

        /// <summary>
        /// Reads the header of any arbitray chunk from the underlying stream.
        /// </summary>
        /// <param name="chunkId">The identifier of the chunk.</param>
        /// <returns>The chunk header.</returns>
        private WaveformFileFormat.ChunkHeader ReadChunkHeader(string chunkId)
        {
            if (_stream.ReadAsString(4) != chunkId)
            {
                throw new InvalidDataException(string.Format("Chunk with id \"{0}\" not found.", chunkId));
            }

            var chunkSize = _stream.ReadAsInt32();

            return new WaveformFileFormat.ChunkHeader(chunkId, chunkSize);
        }
    }
}

