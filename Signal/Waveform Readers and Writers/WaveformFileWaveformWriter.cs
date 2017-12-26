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
    /// Writes a waveform to a waveform file.
    /// </summary>
    public class WaveformFileWaveformWriter : IDisposable
    {
        private readonly Stream _stream;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="stream">A stream accepting the waveform file contents.</param>
        public WaveformFileWaveformWriter (Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="path">The path of the waveform file.</param>
        public WaveformFileWaveformWriter (string path)
            : this(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
        }

        /// <summary>
        /// Writes the waveform to the underlying stream.
        /// </summary>
        public void Write (IWaveform waveForm)
        {
            var formatChunkTotalSize =
                WaveformFileFormat.FormatChunkPayloadSize + WaveformFileFormat.ChunkHeaderSize;
            var dataChunkTotalSize =
                GetDataChunkPayloadSize(waveForm) + WaveformFileFormat.ChunkHeaderSize;
            var rootChunkPayloadSize = WaveformFileFormat.RiffType.Length +
                formatChunkTotalSize + dataChunkTotalSize;

            WriteRootChunkPrologue(rootChunkPayloadSize);
            WriteFormatChunk(waveForm.Format);
            WriteDataChunk(waveForm);
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
        /// Writes the prologue of the root chunk to the underlying stream, i.e. all
        /// bytes up to the first nested chunk.
        /// </summary>
        /// <param name="payloadSize">
        /// The payload size of the root chunk, i.e. including the gross sizes of the
        /// nested chunks.
        /// </param>
        private void WriteRootChunkPrologue(int payloadSize)
        {
            var chunkHeader = new WaveformFileFormat.ChunkHeader(WaveformFileFormat.RootChunkId, payloadSize);
            WriteChunkHeader(chunkHeader);
            _stream.WriteString(WaveformFileFormat.RiffType);
        }

        /// <summary>
        /// Writes the chunk describing the waveform format to the underlying stream.
        /// </summary>
        /// <param name="waveFormFormat">The waveform format.</param>
        private void WriteFormatChunk(WaveformFormat waveFormFormat)
        {
            var chunkSize = WaveformFileFormat.FormatChunkPayloadSize;
            var chunkHeader = new WaveformFileFormat.ChunkHeader(WaveformFileFormat.FormatChunkId, chunkSize);
            WriteChunkHeader(chunkHeader);

            _stream.WriteInt16(WaveformFileFormat.PcmFormatTag);
            _stream.WriteInt16(waveFormFormat.ChannelsCount);
            _stream.WriteInt32(waveFormFormat.SamplesPerSecond);
            _stream.WriteInt32(waveFormFormat.BytesPerSecond);
            _stream.WriteInt16(waveFormFormat.FrameSize);
            _stream.WriteInt16(waveFormFormat.BitsPerSample);
        }

        /// <summary>
        /// Writes the chunk containing the waveform data to the underlying stream.
        /// </summary>
        /// <param name="waveForm">The waveform.</param>
        private void WriteDataChunk(IWaveform waveForm)
        {
            var chunkSize = GetDataChunkPayloadSize(waveForm);
            var chunkHeader = new WaveformFileFormat.ChunkHeader(WaveformFileFormat.DataChunkId, chunkSize);
            WriteChunkHeader(chunkHeader);
            WriteFrames(waveForm);
        }

        /// <summary>
        /// Writes the header of any arbitray chunk to the underlying stream.
        /// </summary>
        /// <param name="chunkHeader">The chunk header.</param>
        private void WriteChunkHeader(WaveformFileFormat.ChunkHeader chunkHeader)
        {
            _stream.WriteString(chunkHeader.Id);
            _stream.WriteInt32(chunkHeader.PayloadSize);
        }

        /// <summary>
        /// Returns the data chunk's payload size for the specified waveform.
        /// </summary>
        /// <param name="waveForm">The waveform.</param>
        /// <returns>The data chunk payload size.</returns>
        private int GetDataChunkPayloadSize(IWaveform waveForm)
        {
            return waveForm.FrameCount * waveForm.Format.FrameSize;
        }

        /// <summary>
        /// Writes the frames of the specified waveform to the underlying the stream.
        /// </summary>
        /// <param name="waveForm">The waveform to write the frames for.</param>
        private void WriteFrames(IWaveform waveForm)
        {
            var format = waveForm.Format;
            var frames = waveForm.GetFrames();

            // TODO support more sample wodths
            switch (format.BitsPerSample)
            {
                case 16:
                    WriteFrames(frames.Cast<WaveForm16BitFrame>(), Write16BitFrame);
                    break;
                default:
                    throw new InvalidDataException(string.Format("Unsupported sample depth ({0} bits/sample).", format.BitsPerSample));
            }
        }

        /// <summary>
        /// Writes the specified frames using the specified frame writer.
        /// </summary>
        private void WriteFrames<TFrame>(IEnumerable<TFrame> frames, Action<TFrame> frameWriter)
            where TFrame : IWaveformFrame
        {
            foreach (var frame in frames)
            {
                frameWriter(frame);
            }
        }

        /// <summary>
        /// Writes the specified 16-bit frame.
        /// </summary>
        private void Write16BitFrame(WaveForm16BitFrame frame)
        {
            foreach(var sample in frame.Samples)
            {
                _stream.WriteInt16(sample);
            }
        }
    }
}

